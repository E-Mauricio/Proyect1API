using Microsoft.EntityFrameworkCore;
using Proyect1API.Models;

namespace Proyect1API.Data
{
    public class ProyectDbContext : DbContext
    {
        public ProyectDbContext(DbContextOptions options) : base(options)
        {

        }

        public ProyectDbContext()
        {

        }

        public virtual DbSet<Product> Products { get; set; }    

        public virtual DbSet<SaleOrder> SaleOrder { get; set; }

        public virtual DbSet<Users> Users { get; set; } 

        public virtual DbSet<ConnectedDevice> ConnectedDevices { get; set; }
    }
}
