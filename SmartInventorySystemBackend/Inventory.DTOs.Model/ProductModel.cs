using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Inventory.DTOs.Model
{
    public class ProductModel
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "The name of product is required")]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive decimal number")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Minimum stock level is required")]
        [Range(0, int.MaxValue, ErrorMessage = "MinStockLevel cannot be negative")]
        public int MinStockLevel { get; set; }

        // --- الحقول الجديدة المضافة ---

        [Range(1, int.MaxValue, ErrorMessage = "Max Capacity must be greater than 0")]
        public int? MaxCapacity { get; set; } // جعلناه اختياري (Nullable)

        public string? ImagePath { get; set; } // مسار الصورة

        // أضف هذا السطر تحديداً لاستقبال الملف المرفوع
        public IFormFile? ImageFile { get; set; }


        // ----------------------------

        [Required]
        public int CategoryID { get; set; }

        public string? CategoryName { get; set; }
    }
}