using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect1API.Data;
using Proyect1API.Models;

namespace Proyect1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ProyectDbContext _ProyectDbContext;

        private readonly IWebHostEnvironment _environment;
        public LoginController(ProyectDbContext proyectDbContext, IWebHostEnvironment environment)
        {
            _ProyectDbContext = proyectDbContext;

            _environment = environment;
        }

        public ProyectDbContext ProyectDbContext { get; }

        //Controllers

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Users newUser)
        {

            await _ProyectDbContext.Users.AddAsync(newUser);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] Users user)
        {
            var existUser = await _ProyectDbContext.Users.FirstOrDefaultAsync(p => p.Email == user.Email && p.Password == user.Password);
            


            return Ok(existUser);  
        }
    }
}
