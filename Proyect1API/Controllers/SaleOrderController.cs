﻿using Microsoft.AspNetCore.Mvc;
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
            public async Task<IActionResult> CreateSO([FromBody] SaleOrder CreateSO)
            {
                await _ProyectDbContext.SaleOrder.AddAsync(CreateSO);

                await _ProyectDbContext.SaveChangesAsync();

                return Ok(CreateSO);
            }

        [HttpGet]
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
        [Route("{orderId:int}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int orderId, SaleOrder updateSaleOrderRequest)
        {
            var updatedSO = await _ProyectDbContext.SaleOrder.FindAsync(orderId);

            if (updatedSO == null)
            {
                return NotFound();
            }
            else
            {
                updatedSO.OrderId = updateSaleOrderRequest.OrderId;

                updatedSO.Address = updateSaleOrderRequest.Address;

                updatedSO.Payment = updateSaleOrderRequest.Payment;

                await _ProyectDbContext.SaveChangesAsync();

                return Ok(updatedSO);
            }               
        }


    }
}

