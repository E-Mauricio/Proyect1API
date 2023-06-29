using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proyect1API.Controllers;
using Proyect1API.Data;
using Proyect1API.Models;
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
            // Create an in-memory database context
            var options = new DbContextOptionsBuilder<ProyectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ProyectDbContext(options);
            _productController = new ProductController(_dbContext, null);
        }

        [Fact]
        public async Task DeviceController_GetAsync_ReturnList()
        {
            // Arrange
            var listOfData = GetData();

            // Add the data to the in-memory database
            _dbContext.ConnectedDevices.AddRange(listOfData);
            _dbContext.SaveChanges();

            // Act
            var response = await _productController.GetAsync();

            // Assert

            //This Assert checks if the response of the Controller is an HTTP 200 OK status code
            var okResult = Assert.IsType<OkObjectResult>(response); 

            //This Assert is validating if okResult.Value can be assign to a List of type ConnectedDevice
            var result = Assert.IsAssignableFrom<List<ConnectedDevice>>(okResult.Value);

            //And here I am validating if the .Count of listOfData is equal to the .Count the var result is returning.
            //It verifies if the number of items in the expected list matches the number of items in the actual list.
            Assert.Equal(listOfData.Count, result.Count);
        }

        private List<ConnectedDevice> GetData()
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
