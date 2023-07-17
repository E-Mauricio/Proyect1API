using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect1API.Data;
using Proyect1API.Models;

namespace Proyect1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleOrderController : Controller
    {

        private readonly ProyectDbContext _ProyectDbContext;

        private readonly IWebHostEnvironment _environment;
        public SaleOrderController(ProyectDbContext proyectDbContext, IWebHostEnvironment environment)
        {
            _ProyectDbContext = proyectDbContext;

            _environment = environment;
        }

        public ProyectDbContext ProyectDbContext { get; }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSO([FromBody] SaleOrder CreateSO)
        {
            await _ProyectDbContext.SaleOrder.AddAsync(CreateSO);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(CreateSO);
        }

        [HttpGet]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSaleOrderById([FromRoute] int id)
        {
            SaleOrder SO = await _ProyectDbContext.SaleOrder.FirstOrDefaultAsync(p => p.OrderId == id);

            if (SO == null)
            {
                return BadRequest(NotFound(id));
            }
            else
            {
                return Ok(SO);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("{orderId:int}")]
        public async Task<IActionResult> UpdateSaleOrder([FromRoute] int orderId, SaleOrder updateSaleOrderRequest)
        {
            var updatedSO = await _ProyectDbContext.SaleOrder.FindAsync(orderId);

            if (updatedSO == null)
            {
                return NotFound();
            }
            else
            {
                //updatedSO.OrderId = updateSaleOrderRequest.OrderId;

                updatedSO.Payment = updateSaleOrderRequest.Payment;

                updatedSO.Address = updateSaleOrderRequest.Address;

                updatedSO.FirstName = updateSaleOrderRequest.FirstName;

                updatedSO.LastName = updateSaleOrderRequest.LastName;

                updatedSO.PostalCode = updateSaleOrderRequest.PostalCode;

                updatedSO.City = updateSaleOrderRequest.City;

                updatedSO.State = updateSaleOrderRequest.State;

                updatedSO.Country = updateSaleOrderRequest.Country;

                updatedSO.Phone = updateSaleOrderRequest.Phone;

                await _ProyectDbContext.SaveChangesAsync();

                return Ok(updatedSO);
            }
        }
    }
}

