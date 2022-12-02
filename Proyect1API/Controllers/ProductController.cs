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

        private readonly IWebHostEnvironment _environment;
        public ProductController(ProyectDbContext proyectDbContext, IWebHostEnvironment environment)
        {
            _ProyectDbContext = proyectDbContext;

            _environment = environment;
        }

        public ProyectDbContext ProyectDbContext { get; }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _ProyectDbContext.Products.ToListAsync();


            if (products != null && products.Count > 0)
            {
                products.ForEach(product =>
                {
                    product.ProductImage = GetImagebyProduct(product.Id);
                });
            }
            else //When it's null.
            {

            }

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

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, Product updateProductRequest)
        {
            var updatedProduct = await _ProyectDbContext.Products.FindAsync(id);

            if (updatedProduct == null)
            {
                return NotFound();
            }

            
            updatedProduct.Name = updateProductRequest.Name;
            updatedProduct.Price = updateProductRequest.Price;  
            updateProductRequest.Quantity = updateProductRequest.Quantity;
            updateProductRequest.ProductImage = updateProductRequest.ProductImage;

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(updatedProduct);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var deletedProduct = await _ProyectDbContext.Products.FindAsync(id);
            
            if (deletedProduct == null)
            {
                return NotFound();
            }

            _ProyectDbContext.Products.Remove(deletedProduct);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(deletedProduct);
        }

        //Images controllers.
        [HttpPost]
        [Route("UploadImage")]
        public async Task<ActionResult> UploadImage()
        {
            bool result = false;

            try
            {
                var _uploadedFile = Request.Form.Files;

                foreach(IFormFile source in _uploadedFile)
                {
                    string fileName = source.FileName;

                    string filePath = GetFilePath(Guid.Parse(fileName));

                    var pId = Path.GetFileNameWithoutExtension(fileName);

                    if(!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }

                    string imagePath = filePath + "\\image.png"; //what about 'fileName'?

                    if(System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await source.CopyToAsync(stream);

                        ////Update image path:
                        var product = _ProyectDbContext.Products.FirstOrDefault(p => p.Id.ToString() == pId);
                        if (product != null)
                        {
                            product.ProductImage = "/Uploads/Product/" + product.Id + "/image.png"; //Change the name?

                            await _ProyectDbContext.SaveChangesAsync();
                        }

                        result = true;
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [NonAction]
        private string GetFilePath(Guid productId)
        {
            return _environment.WebRootPath + "\\Uploads\\Product\\" + productId;
        }

        [NonAction]
        private string GetImagebyProduct(Guid productId)
        {
            string ImageUrl = string.Empty;

            string HostUrl = "https://localhost:7198";

            string filePath = GetFilePath(productId);

            string imagePath = filePath + "\\image.png";

            if(!System.IO.File.Exists(imagePath))
            {
                ImageUrl = HostUrl + "/Uploads/common/noimage.png";
            }
            else
            {
                ImageUrl = HostUrl + "/Uploads/Product/" + productId + "/image.png";
            }
            
            return ImageUrl;

        }

    }
}
