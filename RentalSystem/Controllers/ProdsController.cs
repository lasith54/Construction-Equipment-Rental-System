using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Data;
using RentalSystem.Models;

namespace RentalSystem.Controllers
{
    public class ProdsController : Controller
    {
        private readonly RentalSystemContext _context;

        public ProdsController(RentalSystemContext context)
        {
            _context = context;
        }

        // GET: Prods
        public async Task<IActionResult> Index()
        {
              return _context.Prod != null ? 
                          View(await _context.Prod.ToListAsync()) :
                          Problem("Entity set 'RentalSystemContext.Prod'  is null.");
        }

        // GET: Prods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Prod == null)
            {
                return NotFound();
            }

            var prod = await _context.Prod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }

        // GET: Prods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Prod prod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prod);
        }

        // GET: Prods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Prod == null)
            {
                return NotFound();
            }

            var prod = await _context.Prod.FindAsync(id);
            if (prod == null)
            {
                return NotFound();
            }
            return View(prod);
        }

        // POST: Prods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Prod prod)
        {
            if (id != prod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdExists(prod.Id))
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
            return View(prod);
        }

        // GET: Prods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Prod == null)
            {
                return NotFound();
            }

            var prod = await _context.Prod
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }

        // POST: Prods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Prod == null)
            {
                return Problem("Entity set 'RentalSystemContext.Prod'  is null.");
            }
            var prod = await _context.Prod.FindAsync(id);
            if (prod != null)
            {
                _context.Prod.Remove(prod);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdExists(int id)
        {
          return (_context.Prod?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
