using Castle.Core.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proyect1API.Controllers;
using Proyect1API.Data;
using Proyect1API.Models;
using System.Collections.Generic;
using UnitTest.Tools;

namespace UnitTest
{
    public class UnitTest1
    {
        private readonly Mock<ProyectDbContext> _ProyectDbContext;

        private readonly Mock<IWebHostEnvironment> _environment;
        public UnitTest1()
        {
            _ProyectDbContext = new Mock<ProyectDbContext>();

            _environment = new Mock<IWebHostEnvironment>();
        }
        //[Fact]
        //public void GetProductList_ProductList()
        //{
        //    //arrange
        //    var productList = GetProductsData();
        //    productService.Setup(x => x.GetProductList())
        //        .Returns(productList);
        //    var productController = new ProductController(productService.Object);
        //    //act
        //    var productResult = productController.ProductList();
        //    //assert
        //    Assert.NotNull(productResult);
        //    Assert.Equal(GetProductsData().Count(), productResult.Count());
        //    Assert.Equal(GetProductsData().ToString(), productResult.ToString());
        //    Assert.True(productList.Equals(productResult));
        //}

        [Fact]
        public async Task Test1()
        {
            //arrange
            IQueryable<Product> productList = GetProductsData().AsQueryable();


            var mockSet = productList.GetMockSet<Product>(); // Converts the list to Mock Dbset

            _ProyectDbContext.Setup(x => x.Products)
                .Returns(mockSet.Object);

            //ACT
            ProductController productController = new ProductController(_ProyectDbContext.Object, null);
            //{
            //    ControllerContext = { HttpContext = SetupHttpContext().Object }
            //}; ;

            var actionResult = await productController.GetAllProducts();
            //Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultObject = actionResult.GetObjectResultContent();
            Assert.Equal(productList.Count(), resultObject.Count);

            Assert.Equal("IPhone", resultObject[0].Name);
        }

        private List<Product> GetProductsData()
        {
            List<Product> productsData = new List<Product>
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
            return productsData;
        }
    }
}