using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Proyect1API.Data;
using Proyect1API.Entity;
using Proyect1API.JwtToken;
using Proyect1API.Models;
using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

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
        public virtual async Task<IActionResult> GetAsync()
        {
            var devices = await _ProyectDbContext.ConnectedDevices.ToListAsync();

            return Ok(devices);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _ProyectDbContext.Products.ToListAsync();


            if (products != null && products.Count > 0)
            {
                //products.ForEach(product =>
                //{
                //    product.ProductImage = GetImagebyProduct(product.Id);
                //});
            }
            else //When it's null.
            {
                return NotFound();
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
            else if (product != null)
            {
                product.ProductImage = GetImagebyProduct(product.Id);
            }

            return Ok(product);
        }


        [HttpGet]
        [Route("Search/{name}")]
        public async Task<IActionResult> GetProductByName([FromRoute] string name)
        {
            var products = await _ProyectDbContext.Products.Where(p => p.Name == name).ToListAsync();


            if (products != null && products.Count > 0)
            {
                products.ForEach(product =>
                {
                    product.ProductImage = GetImagebyProduct(product.Id);
                });
            }
            else //When it's null.
            {
                return NotFound();
            }

            return Ok(products);
        }


        //FIX THE IMAGE WHEN UPLOADING A NEW PRODUCT < - - - - - - - - URGENT.!!!!!!!!!!111!!!!111!!!1!!!11
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromBody] Product addProductRequest)
        {
            //How to validate a token claim!!
            //var authToken = new JwtSecurityToken(worker.Token);

            //string tokenRole = authToken.Claims.First(c => c.Type == "Role").Value;


            addProductRequest.Id = Guid.NewGuid(); //Because is 'safer' to create the Id here instead of trusting Angular with that task.

            await _ProyectDbContext.Products.AddAsync(addProductRequest);

            await _ProyectDbContext.SaveChangesAsync();

            return Ok(addProductRequest);

        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [Route("UploadImage")]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)] //This sets the file limit to 100mbs. /Doesn't work.
        public async Task<ActionResult> UploadImage()
        {
            bool result = false;

            try
            {
                var _uploadedFile = Request.Form.Files;

                foreach (IFormFile source in _uploadedFile)
                {
                    string fileName = source.FileName;

                    string filePath = GetFilePath(Guid.Parse(fileName));

                    var pId = Path.GetFileNameWithoutExtension(fileName);

                    if (!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }

                    string imagePath = filePath + "\\image.png"; //what about 'fileName'?

                    //string imagePath = filePath + "\\" + fileName + ".png";

                    if (System.IO.File.Exists(imagePath))
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
                return BadRequest(ex.Message); //This should be addressed.
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("RemoveImage/{id}")]
        public ResponseType RemoveImage(string id)
        {
            string filePath = GetFilePath(new Guid(id));

            string imagePath = filePath + "\\image.png";

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath); //This deletes the img on the product folder.

            }

            return new ResponseType { Result = "pass", KyValue = id };
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

            //string HostUrl = "https://localhost:7198";
            //string HostUrl = "https://localhost:5001";
            string HostUrl = "https://localhost:5000";

            string filePath = GetFilePath(productId);

            string imagePath = filePath + "\\image.png";

            if (!System.IO.File.Exists(imagePath))
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
