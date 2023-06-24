using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Tools
{
    /// <summary>
    /// Extension method that returns a mock dbset of a list
    /// </summary>
    public static class Extensions
    {
        public static Mock<DbSet<T>> GetMockSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns<T>(data.GetEnumerator());

            return mockSet;
        }

        public static T GetObjectResultContent<T>(this ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }
    }
}
