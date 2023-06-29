using Microsoft.EntityFrameworkCore;

namespace Proyect1API.Data
{
    public class MockProyectDbContext : ProyectDbContext
    {
        public MockProyectDbContext() : base(new DbContextOptionsBuilder<ProyectDbContext>().Options)
        {
        }
    }

}
