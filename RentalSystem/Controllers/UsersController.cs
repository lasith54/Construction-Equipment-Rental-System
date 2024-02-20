using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Data;
using RentalSystem.Models;

namespace RentalSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly RentalSystemContext _context;

        public UsersController(RentalSystemContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString, int pg=1)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'user'  is null.");
            }

            IQueryable<User> users = _context.Users;

            if (!String.IsNullOrEmpty(searchString))
            {
                var lowercaseSearchString = searchString.ToLower();
                users = users.Where(s => s.Firstname.ToLower().Contains(lowercaseSearchString) || s.Lastname.ToLower().Contains(lowercaseSearchString));
            }

            const int pageSize = 12;
            int recsCount = await users.CountAsync();
            if (recsCount == 0)
            {
                ViewBag.Message = "No user found.";
                return View();
            }

            if (pg < 1)
            {
                pg = 1;
            }

            int recSkip = (pg - 1) * pageSize;

            if (recSkip < 0)
            {
                recSkip = 0;
            }

            List<User> data = await users.Skip(recSkip).Take(pageSize).ToListAsync();
            Pager pager = new Pager(recsCount, pg, pageSize);
            ViewBag.Pager = pager;

            return View(data);
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            if (int.TryParse(id, out int userId))
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(m => m.UserId == userId);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            else
            {
                return BadRequest();
            }
        }

        public static string HashPassword(string password)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,NIC,Firstname,Lastname,Username,Email,Password,Address,ContactNo")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "User";
                user.Password = HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["UnhashedPassword"] = user.Password;
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,NIC,Firstname,Lastname,Username,Email,Password,Address,ContactNo,Role")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            try
            {
                var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                user.Username = existingUser.Username;
                user.Password = existingUser.Password;
                user.Role = existingUser.Role;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(user); // Update the entity with the modified values
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.UserId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Details));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'RentalSystemContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
