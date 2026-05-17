using Inventory.DataAccess;
using Inventory.DTOs.Model;


namespace Inventory.BusinessLogic
{
    public class clsCategories
    {
        public enum enMode { addNew = 0, update = 1 }
        public enMode Mode = enMode.addNew;

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        public clsCategories()
        {
           
            this.CategoryID = -1;
            this.CategoryName = "";
            this.Mode = enMode.addNew;
        }

        private clsCategories(int CategoryID, string CategoryName)
        {
            this.CategoryID = CategoryID;
            this.CategoryName = CategoryName;
            this.Mode = enMode.update;
        }

        private async Task<bool?> _AddNewCategories()
        {
            // استدعاء طبقة الداتا أكسس
            this.CategoryID =(int) await clsCategoriesData.AddNewCategories(this.CategoryName);
            return (this.CategoryID != -1);
        }

        public static async Task<bool> Delete(int CategoryID)
        {
            return await clsCategoriesData.DeleteCategories(CategoryID);
        }

        // ملاحظة: يفضل جعل الـ Find أيضاً Async إذا كانت قاعدة البيانات تدعم ذلك
        public async  static Task<CategoryModel?> Find(int CategoryID)
        {
            return await clsCategoriesData.FindByID(CategoryID);

        }

        //public static clsCategories FindByName(string CategoryName)
        //{
        //    int CategoryID = -1;
        //    // تصحيح: إزالة الفاصلة الزائدة في النهاية
        //    bool IsFound = clsCategoriesData.FindByName(ref CategoryID, CategoryName);

        //    if (IsFound)
        //        return new clsCategories(CategoryID, CategoryName);

        //    return null;
        //}

        public static async Task<List<CategoryModel>> GetAllCategories()
        {
            return await clsCategoriesData.GetAllCategories();
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case enMode.addNew:
                    if ((bool)await _AddNewCategories())
                    {
                        Mode = enMode.update; // نغير الوضع بعد النجاح
                        return true;
                    }
                    return false;

                case enMode.update:
                    return await _UpdateCategories();
            }
            return false;
        }

        private async Task<bool> _UpdateCategories()
        {
            // تصحيح: التعامل مع القيمة الراجعة بناءً على تصميم الـ Data Layer
            return await clsCategoriesData.UpdateCategories(this.CategoryID, this.CategoryName);
        }
    }
}
