using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Proyect1API.Controllers;
using Proyect1API.Data;
using Proyect1API.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Tools;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        private readonly Mock<ProyectDbContext> _dbContext;
        private ProductController _productController;

        public UnitTest1()
        {
            _dbContext = new Mock<ProyectDbContext>();
        }

        [Fact]
        public async void DeviceController_GetAsync_ReturnList()
        {
            //arrange
            var deviceList = GetTestData().AsQueryable();


            //var x3 = x1.ToListAsync();  
            var mockSet = deviceList.GetMockSet(); // Converts the list to Mock Dbset

            _dbContext.Setup(x => x.ConnectedDevices).ReturnsDbSet(GetTestData());
            //.ReturnsDbSet(new List<Product>() { new Product()}.AsQueryable().GetMockSet().Object);

            _productController = new ProductController(_dbContext.Object, null);

            //Act
            var result = await _productController.GetAsync();

            //Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;

            var convertedObject = Assert.IsAssignableFrom<List<ConnectedDevice>>(okResult.Value);

            Assert.Equal(GetTestData().Count, convertedObject.Count);
            Assert.Equal("9876", convertedObject.First().DeviceId);

            Assert.NotNull(convertedObject);
        }

        private List<ConnectedDevice> GetTestData()
        {
            List<ConnectedDevice> data = new List<ConnectedDevice>();

            var device1 = new ConnectedDevice();
            device1.PairedDeviceId = "1234";
            device1.DeviceName = "test1";
            device1.DeviceId = "9876";
            data.Add(device1);

            var device2 = new ConnectedDevice();
            device2.PairedDeviceId = "5678";
            device2.DeviceName = "test2";
            device2.DeviceId = "5432";
            data.Add(device2);

            return data;
        }
    }
}
