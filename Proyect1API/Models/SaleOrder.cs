using System.ComponentModel.DataAnnotations;

namespace Proyect1API.Models
{
    public class SaleOrder
    {
        [Key]
        public int OrderId { get; set; }
        public string Payment { get; set; }
        public string Address { get; set; } 
    }
}
