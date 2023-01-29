using System.ComponentModel.DataAnnotations;

namespace Proyect1API.Models
{
    public class SaleOrder
    {
        [Key]
        public int OrderId { get; set; }
        public string Payment { get; set; }
        public string Address { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }   
    }
}
