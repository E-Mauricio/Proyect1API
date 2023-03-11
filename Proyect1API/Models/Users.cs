using System.ComponentModel.DataAnnotations;

namespace Proyect1API.Models
{
    public class Users
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
