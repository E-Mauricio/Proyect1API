﻿using Microsoft.EntityFrameworkCore;
using Proyect1API.Models;

namespace Proyect1API.Data
{
    public class ProyectDbContext : DbContext
    {
        public ProyectDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }    

        public DbSet<SaleOrder> SaleOrder { get; set; }

        public DbSet<Users> Users { get; set; } 
    }
}
