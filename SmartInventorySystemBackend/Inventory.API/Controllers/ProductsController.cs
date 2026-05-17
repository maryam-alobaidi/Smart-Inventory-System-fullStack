using Inventory.BusinessLogic;
using Inventory.DTOs.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
        {
            var Products = await clsProducts.GetAllProducts();
            return Ok(Products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            var Product = await clsProducts.Find(id);
            if (Product == null)
            {
                return NotFound($"Product with {id} not found.");
            }
            return Ok(Product);
        }

        [HttpPost]
        // أضفنا [FromForm] لاستقبال البيانات، و IFormFile لاستقبال الصورة
        public async Task<ActionResult<ProductModel>> Add([FromForm] ProductModel product, IFormFile? imageFile)
        {
            clsProducts newProduct = new clsProducts();
            newProduct.ProductName = product.ProductName;
            newProduct.Quantity = product.Quantity;
            newProduct.Price = product.Price;
            newProduct.MaxCapacity = product.MaxCapacity;
            newProduct.MinStockLevel = product.MinStockLevel;
            newProduct.CategoryID = product.CategoryID;

            // --- كود معالجة الصورة وحفظها في السيرفر ---
            if (imageFile != null && imageFile.Length > 0)
            {
                // 1. تحديد مكان الحفظ (مثلاً مجلد wwwroot/uploads)
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/products");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                // 2. إنشاء اسم فريد للملف لمنع التكرار
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(uploadsPath, fileName);

                // 3. حفظ الملف فعلياً في القرص الصلب للسيرفر
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // 4. تخزين المسار في قاعدة البيانات
                newProduct.ImagePath = "products/" + fileName;
            }
            // ----------------------------------------

            if (!await newProduct.Save())
            {
                return BadRequest("Failed to add new product.");
            }
            else
            {
                product.ProductID = newProduct.ProductID;
                // أعد المسار الجديد في الـ Model ليظهر في الأنجلر
                product.ImagePath = newProduct.ImagePath;
                return CreatedAtAction(nameof(GetById), new { id = product.ProductID }, product);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] ProductModel product)
        {
            if (id != product.ProductID)
            {
                return BadRequest("Product ID mismatch.");
            }

            clsProducts clsProducts = new clsProducts();
            clsProducts.ProductID = product.ProductID;
            clsProducts.ProductName = product.ProductName;
            clsProducts.Quantity = product.Quantity;
            clsProducts.Price = product.Price;
            clsProducts.MinStockLevel = product.MinStockLevel;
            clsProducts.MaxCapacity = product.MaxCapacity;
            clsProducts.CategoryID = product.CategoryID;
            clsProducts.Mode = clsProducts.enMode.update;

            // --- الجزء الخاص بمعالجة الملف المرفوع ---
            if (product.ImageFile != null) // نتحقق من الملف الجديد
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // نستخدم ImageFile.FileName بدلاً من ImagePath.FileName
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream); // ننسخ الملف الحقيقي
                }

                // هنا نخزن المسار النصي الجديد في قاعدة البيانات
                clsProducts.ImagePath = "Images/" + uniqueFileName;
            }
            else
            {
                // إذا لم يرفع صورة، نأخذ المسار القديم الموجود في ImagePath
                clsProducts.ImagePath = product.ImagePath;
            }
            // ---------------------------------------

            if (!await clsProducts.Save())
                {
                    return BadRequest("Failed to update product.");
                }

                // يفضل إرجاع المسار الجديد ليتمكن الأنجلر من تحديث الصورة فوراً
                return Ok(new { imagePath = clsProducts.ImagePath });
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await clsProducts.Delete(id))
            {
                return NoContent();
            }
            else
            {
                return BadRequest($"Failed to delete product with id {id}.");
            }
        }
    }
}