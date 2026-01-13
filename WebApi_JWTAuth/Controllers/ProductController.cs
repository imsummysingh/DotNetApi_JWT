using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi_JWTAuth.Data;

namespace WebApi_JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetProducts")]
        public ActionResult GetProducts() 
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id) 
        {
            var product = _context.Products.Find(id);
            if (product == null) 
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
