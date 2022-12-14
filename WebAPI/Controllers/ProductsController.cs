using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Classes;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiVersion("1.0")]
    //[Route("/v{v:apiVersion}/products")]
    [Route("products")]
    [ApiController]
    public class ProductsV1_0Controller : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsV1_0Controller(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();      //actually runs the seeding
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<Product>> GetAllProducts()
        //{
        //    var products = _context.Products.ToArray();
        //    return Ok(products);
        //    //return Ok(_context.Products.ToArray());
        //}

        //[HttpGet]  //IEnumerable return
        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return _context.Products.ToArray();
        //}

        //[HttpGet("{id}")]
        ////[HttpGet, Route("/products/{id}")]
        //public ActionResult GetProduct(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    if(product == null)
        //        return NotFound();
        //    return Ok(product);
        //}

        //Async API
        //[HttpGet]
        //public async Task<IEnumerable<Product>> GetAllProducts()
        //{
        //    return await _context.Products.ToArrayAsync();
        //}

        //Paginating Items
        //[HttpGet]
        //public async Task<IActionResult> GetAllProducts([FromQuery] QueryParameters queryParameters)
        //{
        //    IQueryable<Product> products = _context.Products;
        //    products = products
        //        .Skip(queryParameters.Size * (queryParameters.Page - 1))
        //        .Take(queryParameters.Size);

        //    return Ok(await products.ToArrayAsync());
        //}

        //Filtering, Searching & Sorting

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;

            if (queryParameters.MinPrice != null &&
                queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value &&
                         p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products.Where(p => p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                                               p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _context.Entry(product).State = EntityState.Modified; // Need to know more details
            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete ("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        //Delete Several items
        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<Product>> DeleteMultipleProduct([FromQuery]int[] ids)
        {
            var products = new List<Product>();
            foreach (int id in ids)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
    }
    //[ApiVersion("2.0")]
    //[Route("v{v:apiVersion}/products")]
    ////[Route("products")]
    //[ApiController]
    //public class ProductsV2_0Controller : ControllerBase
    //{
    //    private readonly ShopContext _context;
    //    public ProductsV2_0Controller(ShopContext context)
    //    {
    //        _context = context;
    //        _context.Database.EnsureCreated();      //actually runs the seeding
    //    }

    //    //[HttpGet]
    //    //public ActionResult<IEnumerable<Product>> GetAllProducts()
    //    //{
    //    //    var products = _context.Products.ToArray();
    //    //    return Ok(products);
    //    //    //return Ok(_context.Products.ToArray());
    //    //}

    //    //[HttpGet]  //IEnumerable return
    //    //public IEnumerable<Product> GetAllProducts()
    //    //{
    //    //    return _context.Products.ToArray();
    //    //}

    //    //[HttpGet("{id}")]
    //    ////[HttpGet, Route("/products/{id}")]
    //    //public ActionResult GetProduct(int id)
    //    //{
    //    //    var product = _context.Products.Find(id);
    //    //    if(product == null)
    //    //        return NotFound();
    //    //    return Ok(product);
    //    //}

    //    //Async API
    //    //[HttpGet]
    //    //public async Task<IEnumerable<Product>> GetAllProducts()
    //    //{
    //    //    return await _context.Products.ToArrayAsync();
    //    //}

    //    //Paginating Items
    //    //[HttpGet]
    //    //public async Task<IActionResult> GetAllProducts([FromQuery] QueryParameters queryParameters)
    //    //{
    //    //    IQueryable<Product> products = _context.Products;
    //    //    products = products
    //    //        .Skip(queryParameters.Size * (queryParameters.Page - 1))
    //    //        .Take(queryParameters.Size);

    //    //    return Ok(await products.ToArrayAsync());
    //    //}

    //    //Filtering, Searching & Sorting

    //    [HttpGet]
    //    public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
    //    {
    //        IQueryable<Product> products = _context.Products.Where(p => p.IsAvailable == true);

    //        if (queryParameters.MinPrice != null &&
    //            queryParameters.MaxPrice != null)
    //        {
    //            products = products.Where(
    //                p => p.Price >= queryParameters.MinPrice.Value &&
    //                     p.Price <= queryParameters.MaxPrice.Value);
    //        }

    //        if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
    //        {
    //            products = products.Where(p => p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
    //                                           p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
    //        }

    //        if (!string.IsNullOrEmpty(queryParameters.Sku))
    //        {
    //            products = products.Where(p => p.Sku == queryParameters.Sku);
    //        }

    //        if (!string.IsNullOrEmpty(queryParameters.Name))
    //        {
    //            products = products.Where(
    //                p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
    //        }

    //        if (!string.IsNullOrEmpty(queryParameters.SortBy))
    //        {
    //            if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
    //            {
    //                products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
    //            }
    //        }

    //        products = products
    //            .Skip(queryParameters.Size * (queryParameters.Page - 1))
    //            .Take(queryParameters.Size);

    //        return Ok(await products.ToArrayAsync());
    //    }

    //    [HttpGet("{id}")]
    //    public async Task<ActionResult> GetProduct(int id)
    //    {
    //        var product = await _context.Products.FindAsync(id);
    //        if (product == null)
    //        {
    //            return NotFound();
    //        }
    //        return Ok(product);
    //    }

    //    [HttpPost]
    //    public async Task<ActionResult<Product>> PostProduct(Product product)
    //    {
    //        //if (!ModelState.IsValid)
    //        //{
    //        //    return BadRequest();
    //        //}
    //        _context.Products.Add(product);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction(
    //            "GetProduct",
    //            new { id = product.Id },
    //            product);
    //    }
    //    [HttpPut("{id}")]
    //    public async Task<ActionResult> PutProduct(int id, Product product)
    //    {
    //        if (id != product.Id)
    //        {
    //            return BadRequest();
    //        }
    //        _context.Entry(product).State = EntityState.Modified; // Need to know more details
    //        try
    //        {
    //            _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!_context.Products.Any(p => p.Id == id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }

    //        return NoContent();
    //    }
    //    [HttpDelete("{id}")]
    //    public async Task<ActionResult<Product>> DeleteProduct(int id)
    //    {
    //        var product = await _context.Products.FindAsync(id);
    //        if (product == null)
    //        {
    //            return NotFound();
    //        }
    //        _context.Products.Remove(product);
    //        await _context.SaveChangesAsync();

    //        return product;
    //    }

    //    //Delete Several items
    //    [HttpPost]
    //    [Route("Delete")]
    //    public async Task<ActionResult<Product>> DeleteMultipleProduct([FromQuery] int[] ids)
    //    {
    //        var products = new List<Product>();
    //        foreach (int id in ids)
    //        {
    //            var product = await _context.Products.FindAsync(id);
    //            if (product == null)
    //            {
    //                return NotFound();
    //            }
    //            products.Add(product);
    //        }

    //        _context.Products.RemoveRange(products);
    //        await _context.SaveChangesAsync();

    //        return Ok(products);
    //    }
    //}
}
