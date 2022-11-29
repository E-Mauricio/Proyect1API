using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect1API.Data;
using Proyect1API.Models;

namespace Proyect1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ProyectDbContext _ProyectDbContext;
        public ProductController(ProyectDbContext proyectDbContext)
        {
            _ProyectDbContext = proyectDbContext;
        }

        public ProyectDbContext ProyectDbContext { get; }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _ProyectDbContext.Products.ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var product = await _ProyectDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        } 
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product addProductRequest)
        {
            addProductRequest.Id = Guid.NewGuid(); //Because is 'safer' to create the Id here instead of trusting Angular with that task.

            await _ProyectDbContext.Products.AddAsync(addProductRequest);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(addProductRequest);
        }

    }
}
