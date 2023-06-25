using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proyect1API.Controllers;
using Proyect1API.Data;
using Proyect1API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        private readonly ProyectDbContext _dbContext;
        private readonly ProductController _productController;

        public UnitTest1()
        {
            // Create an in-memory database
            var options = new DbContextOptionsBuilder<ProyectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ProyectDbContext(options);

            // Populate the in-memory database with test data
            _dbContext.Products.AddRange(GetProductsData());
            _dbContext.SaveChanges();

            _productController = new ProductController(_dbContext, null);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsProductList()
        {
            // Act
            var result = await _productController.GetAllProducts();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult.Value);

            var products = okResult.Value as IEnumerable<Product>;
            Assert.Equal(GetProductsData().Count, products.Count());
            Assert.Equal("IPhone", products.First().Name);
        }

        private List<Product> GetProductsData()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "IPhone",
                    Price = 55000,
                    Quantity = 10,
                    ProductImage = "c:/code/c/boook"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "IPhone2",
                    Price = 55600,
                    Quantity = 10,
                    ProductImage = "c:/code/c/boook"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "IPhone3",
                    Price = 57000,
                    Quantity = 110,
                    ProductImage = "c:/code/c/boook"
                },
            };
        }
    }
}
