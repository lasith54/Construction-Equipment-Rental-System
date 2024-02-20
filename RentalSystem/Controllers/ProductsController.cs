using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Data;
using RentalSystem.Models;


using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace RentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly RentalSystemContext _context;

        public ProductsController(RentalSystemContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            webHostEnvironment = webHost;
        }

        // GET: Products
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
            if(recsCount == 0)
            {
                ViewBag.Message = "No products found.";
                return View();
            }

            int totalPages = (int)Math.Ceiling((double)recsCount /pageSize);

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

        // GET: Products/Details/5
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

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        private string? UploadedFile(Product product)
        {
            string? uniqueFileName = null;

            if (product.ProductImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() +"_" + product.ProductImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    product.ProductImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Productname,Manufacturer,Description,Rent,Quantity,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = UploadedFile(product);
                product.ImageUrl = uniqueFileName;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.ImageUrl = Url.Content("../images/"+product.ImageUrl);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Productname,Manufacturer,Description,Rent,Quantity,ProductImage")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string? uniqueFileName = UploadedFile(product);
                    if (uniqueFileName != null)
                    {
                        product.ImageUrl = uniqueFileName;
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'RentalSystemContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
