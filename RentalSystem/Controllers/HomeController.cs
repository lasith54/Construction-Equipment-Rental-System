using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using RentalSystem.Data;
using RentalSystem.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RentalSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RentalSystemContext _context;

        public HomeController(ILogger<HomeController> logger, RentalSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int pg=1)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'product'  is null.");
            }

            IQueryable<Product> products = _context.Product;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => EF.Functions.Like(s.Productname, $"%{searchString}%") || EF.Functions.Like(s.Manufacturer, $"%{searchString}%"));
            }

            const int pageSize = 12;
            int recsCount = await products.CountAsync();
            if (recsCount == 0)
            {
                ViewBag.Message = "No products found.";
                return View();
            }

            int totalPages = (int)Math.Ceiling((double)recsCount / pageSize);

            if (pg < 1)
            {
                pg = 1;
            }
            int recSkip = (pg - 1) * pageSize;

            if (recSkip < 0)
            {
                recSkip = 0;
            }

            List<Product> data = await products.Skip(recSkip).Take(pageSize).ToListAsync();
            ViewBag.Pager = new Pager(recsCount, pg, pageSize);

            //return View(await products.ToListAsync());
            return View(data);
            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}