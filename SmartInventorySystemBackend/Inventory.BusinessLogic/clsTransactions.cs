using Inventory.DataAccess;
using Inventory.DTOs.Model;


namespace Inventory.BusinessLogic
{
    public class clsTransactions
    {
        public enum enMode { addNew = 0, update = 1 }
        public enMode Mode = enMode.addNew;



        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public enTransactionType Type { get; set; }//"in" for adding stock, "out" for removing stock
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public clsTransactions()
        {
            this.TransactionID = -1;
            this.ProductID = -1;
            this.UserID = -1;
            this.Type = enTransactionType.In;
            this.Quantity = -1;
            this.TransactionDate = DateTime.MinValue;
            this.Mode = enMode.addNew;
        }

        private clsTransactions(int TransactionID, int ProductID, int UserID, int Type, int Quantity, DateTime TransactionDate)
        {
            this.TransactionID = TransactionID;
            this.ProductID = ProductID;
            this.UserID = UserID;
            this.Type = (enTransactionType)Type;
            this.Quantity = Quantity;
            this.TransactionDate = TransactionDate;

            this.Mode = enMode.update;
        }

        private async Task<bool> _AddNewTransactions()
        {
            var product = await clsProducts.Find(this.ProductID); // Find the product by ID to get the current quantity and other details of the
            if (product == null)
            {
                return false; // Product not found, cannot update transaction
            }


            int newStockQuantity = (this.Type == enTransactionType.In ?
            product.Quantity + this.Quantity :
            product.Quantity - this.Quantity);

            if (this.Type == enTransactionType.Out && product.Quantity > 0)
            {
                if ((product.Quantity-product.MinStockLevel) < this.Quantity)
                {
                    throw new Exception($"Insufficient stock! Current: {product.Quantity}, Removing: {this.Quantity} The Min Stock Level:{product.MinStockLevel} ");
                }
            }
            

            int totalExpectedQuantity = product.Quantity + this.Quantity;
            if (this.Type == enTransactionType.In && product.MaxCapacity.HasValue)
            {
                if (totalExpectedQuantity > product.MaxCapacity.Value)
                {
                    throw new Exception($"Store Full! Current: {product.Quantity}, Adding: {this.Quantity}, Max allowed is: {product.MaxCapacity.Value}");
                }
            }
            int transactionID = await clsTransactionsData.AddNewTransactions(this.ProductID, this.UserID, this.Type, this.Quantity, this.TransactionDate) ?? -1;
            if (transactionID > 0)
            {
                this.TransactionID = transactionID;
                bool isProductUpdated = await clsProductsData.UpdateProductQuantity(this.ProductID, newStockQuantity);

                if (!isProductUpdated)
                    throw new Exception("Transaction saved, but failed to update product stock!");

                return true;
            }
            return false;
        }

        public static Task<bool> Delete(int TransactionID)
        {
            // Call DataAccess Layer
            return clsTransactionsData.DeleteTransactions(TransactionID);
        }

        public async static Task<TransactionModel?> Find(int TransactionID)
        {

            TransactionModel? transactionModel =await clsTransactionsData.FindByID(TransactionID);
           
            return transactionModel;
        }


        public static async Task<List<TransactionModel?>> GetAllTransactions()
        {
            return await clsTransactionsData.GetAllTransactions();
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case enMode.addNew:
                    Mode = enMode.update;
                    return await _AddNewTransactions();
                case enMode.update:
                    return await _UpdateTransactions();
            }
            return false;
        }
        ///update the transaction and update the quantity of the product accordingly at here i will find the product by id and update the quantity of the product according to the type of transaction
        private async Task<bool> _UpdateTransactions()
        {

            var oldTansaction = await clsTransactions.Find(this.TransactionID);

            if (oldTansaction == null)
            {
                return false; // Transaction not found, cannot update
            }

            var product = await clsProducts.Find(this.ProductID); // Find the product by ID to get the current quantity and other details of the
            if (product == null)
            {
                return false;
            }

            int currentQuantity = product.Quantity;

            if (this.Type == enTransactionType.In)
            {
                currentQuantity -= oldTansaction.Quantity; // Remove the old quantity from the current quantity
                currentQuantity += this.Quantity; // Add the new quantity to the current quantity
            }
            else
            {
                currentQuantity += oldTansaction.Quantity; // Add the old quantity back to the current quantity
                currentQuantity -= this.Quantity; // Remove the new quantity from the current quantity
            }


            if (currentQuantity < 0)
                throw new Exception("Insufficient stock!");


            if(this.Type == enTransactionType.In && product.MaxCapacity.HasValue && currentQuantity > product.MaxCapacity.Value)
                throw new Exception($"Cannot add items. Total quantity ({currentQuantity}) will exceed maximum capacity ({product.MaxCapacity}).");


            // In _UpdateTransactions(), change the call to clsTransactionsData.UpdateTransactions to pass Type.ToString() instead of Type
            bool isUpdate = await clsTransactionsData.UpdateTransactions(
                this.TransactionID,
                this.ProductID,
                this.UserID,
                this.Type,
                this.Quantity,
                this.TransactionDate);

            if (isUpdate)
            {
              return  await clsProductsData.UpdateProductQuantity(this.ProductID, currentQuantity);
            }
            return false;
        }


    }

}
