using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.DTOs.Model
{
    public enum enTransactionType
    {
        In = 1,
        Out = 2
    }
    public class TransactionModel
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int ProductID { get; set; }
        [Required]
        public int UserID { get; set; }

        [Required (ErrorMessage = "The Type of transaction is required")]

        public enTransactionType Type { get; set; }//"in" for adding stock, "out" for removing stock

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer")]
        public int Quantity { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }=DateTime.Now; 

        public ProductModel? Product { get; set; }

        public UserModel? User { get; set; }

    }
}
