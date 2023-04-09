using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyect1API.Data;
using Proyect1API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyect1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ProyectDbContext _ProyectDbContext;

        private readonly IWebHostEnvironment _environment;

        IConfiguration Configuration;      
        public LoginController(ProyectDbContext proyectDbContext, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _ProyectDbContext = proyectDbContext;

            _environment = environment;

            Configuration = configuration;

        }

        public ProyectDbContext ProyectDbContext { get; }

        //Controllers

        [HttpPost]
        [Route("createuser")]
        public async Task<IActionResult> CreateUser([FromBody] Users newUser)
        {

            var newlyCreatedUser = newUser;

            await _ProyectDbContext.Users.AddAsync(newlyCreatedUser);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(newlyCreatedUser);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] Users user)
        {
            var existUser = await _ProyectDbContext.Users.FirstOrDefaultAsync(p => p.Email == user.Email && p.Password == user.Password);

            if (existUser != null)
            {
                var issuer = Configuration["Jwt:Issuer"];
                var audience = Configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes
                (Configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, existUser.Email),
                new Claim(JwtRegisteredClaimNames.Email, existUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
                    Expires = DateTime.UtcNow.AddMinutes(5), //How long will the token be valid.
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);

                existUser.Token = stringToken;

                await _ProyectDbContext.SaveChangesAsync();

                return Ok(existUser);
            }
            return NotFound("Email or Password are incorrect. Try Again.");

        }

        [HttpGet]
        public async Task<IActionResult> Sample(string username, string password)
        {
            var products = await _ProyectDbContext.Products.ToListAsync();

            if (products != null)
            {
                return Ok(products);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
