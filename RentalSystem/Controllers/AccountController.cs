using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Data;
using RentalSystem.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RentalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly RentalSystemContext _context;

        public AccountController(RentalSystemContext context) 
        {
            _context = context;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        public IActionResult Login(string returnUrl="/")
        {
            Login loginModel = new Login();
            loginModel.ReturnUrl = returnUrl;
            return View(loginModel);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
                string hashedProvidedPassword = Convert.ToBase64String(hashedBytes);
                return hashedPassword.Equals(hashedProvidedPassword);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
            //RentalSystemContext userContext = new RentalSystemContext();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginModel.UserName);
            if(user!=null && VerifyHashedPassword(user.Password, loginModel.Password))
            {
                var claims = new List<Claim>();
                if (user.UserId != null)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserId)));
                }

                if (!string.IsNullOrEmpty(user.Username))
                {
                    claims.Add(new Claim(ClaimTypes.Name, user.Username));
                }

                if (!string.IsNullOrEmpty(user.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                }

                claims.Add(new Claim("Admin", "Code"));
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal,
                    new AuthenticationProperties()
                    {
                        IsPersistent = loginModel.RememberLogin,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    });
                return LocalRedirect(loginModel.ReturnUrl);
            }
            else
            {
                ViewBag.Message = "Invalid Credential";
                return View(loginModel);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }
    }
}
