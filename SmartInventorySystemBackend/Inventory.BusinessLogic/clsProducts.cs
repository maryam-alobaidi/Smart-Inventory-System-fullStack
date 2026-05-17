using Inventory.DataAccess;
using Inventory.DTOs.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.BusinessLogic
{
    public class clsProducts
    {
        public enum enMode { addNew = 0, update = 1 }
        public enMode Mode = enMode.addNew;

        // الحقول الأساسية + الحقول الذكية الجديدة
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int MinStockLevel { get; set; }
        public int? MaxCapacity { get; set; } // الحقل الجديد (الذكاء البرمجي)
        public string? ImagePath { get; set; } // الحقل الجديد (الصور)
        public int CategoryID { get; set; }

        public clsProducts()
        {
            this.ProductID = -1;
            this.ProductName = "";
            this.Quantity = 0;
            this.Price = 0.0m;
            this.MinStockLevel = 0;
            this.MaxCapacity = null;
            this.ImagePath = null;
            this.CategoryID = -1;
            this.Mode = enMode.addNew;
        }

        // Constructor للتحديث (يستقبل الحقول الجديدة)
        private clsProducts(int ProductID, string ProductName, int Quantity, decimal Price,
                            int MinStockLevel, int? MaxCapacity, string? ImagePath, int CategoryID)
        {
            this.ProductID = ProductID;
            this.ProductName = ProductName;
            this.Quantity = Quantity;
            this.Price = Price;
            this.MinStockLevel = MinStockLevel;
            this.MaxCapacity = MaxCapacity;
            this.ImagePath = ImagePath;
            this.CategoryID = CategoryID;

            this.Mode = enMode.update;
        }

        private async Task<bool> _AddNewProducts()
        {
            // تحديث الاستدعاء ليتوافق مع الـ Stored Procedure الجديد
            this.ProductID = (int)await clsProductsData.AddNewProducts(
                this.ProductName,
                this.Quantity,
                this.Price,
                this.MinStockLevel,
                this.MaxCapacity ?? 0, // تمرير 0 كقيمة افتراضية إذا كانت Null
                this.ImagePath ?? "",
                this.CategoryID);

            return (this.ProductID != -1);
        }

        private async Task<bool> _UpdateProducts()
        {
            // تحديث الاستدعاء ليتوافق مع الـ Stored Procedure الجديد
            return await clsProductsData.UpdateProducts(
                this.ProductID,
                this.ProductName,
                this.Quantity,
                this.Price,
                this.MinStockLevel,
                this.MaxCapacity ?? 0,
                this.ImagePath ?? "",
                this.CategoryID) ?? false;
        }

      
        public static Task<bool> Delete(int ProductID)
        {
            // Call DataAccess Layer
            return clsProductsData.DeleteProducts(ProductID);
        }

        public async static Task<ProductModel?> Find(int ProductID)
        {
            ProductModel? productModel =await clsProductsData.FindById(ProductID);
            return productModel;
        }

        //public static clsProducts FindByName(string ProductName)
        //{
        //    // Call DataAccess Layer
        //    int ProductID = -1;
        //    int Quantity = -1;
        //    decimal Price = 0.0m;
        //    int MinStockLevel = -1;
        //    int CategoryID = -1;

        //    bool IsFound = clsProductsData.FindByName(ref ProductID, ProductName, ref Quantity, ref Price, ref MinStockLevel, ref CategoryID);
        //    if (IsFound)
        //        return new clsProducts(ProductID, ProductName, Quantity, Price, MinStockLevel, CategoryID);
        //    else
        //        return null;
        //}

        public static async Task<List<ProductModel>> GetAllProducts()
        {
            return await clsProductsData.GetAllProducts();
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case enMode.addNew:
                    Mode = enMode.update;
                    return await _AddNewProducts();
                case enMode.update:
                    return await _UpdateProducts();
            }
            return false;
        }

     
        public static async Task<bool> UpdateProductQuantity(int productID, int quentity)
        {
            return await clsProductsData.UpdateProductQuantity(productID, quentity);
        }

    }

}
