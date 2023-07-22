using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApiTest.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int customerId { get; set; }
        [Required]
        public string customerCode { get; set; }
        [Required]
        public string customerName { get; set; }
        public string customerAddress { get; set; }

        public int createdBy { get; set; }
        public DateTime createdAt { get; set; }

        public int? modifiedBy { get; set; } = null;
        public DateTime? modifiedAt { get; set; } = null;
    }
}