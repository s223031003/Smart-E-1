using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart_E.Models
{
    public class TransactionsModel
    {
        [Key]
        public int TransactionId { get; set; }
        [MaxLength(12)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Account Number")]
        [Column(TypeName = "nvarchar(12)")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Account Owner")]
        [Column(TypeName = "nvarchar(100)")]
        public string AccountOwner { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Bank Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string BankName { get; set; }
        [MaxLength(3)]
        [Required(ErrorMessage = "This field is required.")]
        [Column(TypeName = "nvarchar(3)")]
        public string CVV { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public int Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

    }
}