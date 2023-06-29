using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proyect1API.Controllers;
using Proyect1API.Data;
using Proyect1API.Models;
using System;
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
        private readonly ProductController _productController;

        public UnitTest1()
        {
            // Create a mock DbSet
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ConnectedDevice>>();

            // Configure mock DbSet to return test data
            mockSet.As<IQueryable<ConnectedDevice>>()
                .Setup(m => m.Provider)
                .Returns(GetTestData().AsQueryable().Provider);
            mockSet.As<IQueryable<ConnectedDevice>>()
                .Setup(m => m.Expression)
                .Returns(GetTestData().AsQueryable().Expression);
            mockSet.As<IQueryable<ConnectedDevice>>()
                .Setup(m => m.ElementType)
                .Returns(GetTestData().AsQueryable().ElementType);
            mockSet.As<IQueryable<ConnectedDevice>>()
                .Setup(m => m.GetEnumerator())
                .Returns(GetTestData().AsQueryable().GetEnumerator());

            // Configure mock DbSet to support ToListAsync
            mockSet.As<IAsyncEnumerable<ConnectedDevice>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ConnectedDevice>(GetTestData().GetEnumerator()));

            // Create a mock DbContext and set up the behavior
            _dbContext = new Mock<ProyectDbContext>();
            _dbContext.Setup(db => db.ConnectedDevices).Returns(mockSet.Object);

            _productController = new ProductController(_dbContext.Object, null);
        }

        [Fact]
        public async Task DeviceController_GetAsync_ReturnList()
        {
            var result = await _productController.GetAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<ConnectedDevice>>(okResult.Value);


        }

        private List<ConnectedDevice> GetTestData()
        {
            List<ConnectedDevice> data = new List<ConnectedDevice>();

            var device = new ConnectedDevice();

            device.PairedDeviceId = "1234";
            device.DeviceName = "test";
            device.DeviceId = "9876";

            data.Add(device);

            device.PairedDeviceId = "1234";
            device.DeviceName = "test";
            device.DeviceId = "9876";

            data.Add(device);

            return data;

        }
    }
}
