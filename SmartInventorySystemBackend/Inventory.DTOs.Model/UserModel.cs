
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Inventory.DTOs.Model
{
        public class UserModel
        {
            [Key]
            public int UserID { get; set; }

            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(255)] // طول كبير لتخزين كلمة المرور بعد التشفير (Hashing)
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "User role is required")]
            [StringLength(20)]
            public string Role { get; set; } = string.Empty; // مثل: Admin, Cashier, Manager

          
        }
    }
