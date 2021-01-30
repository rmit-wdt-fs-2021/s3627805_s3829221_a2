using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InternetBankingWebApp.Data;
using InternetBankingWebApp.Models;
using Microsoft.EntityFrameworkCore;
using SimpleHashing;

namespace InternetBankingWebApp.Controllers
{
    [Route("/Mcba/MyLogin")]
    public class LoginController : Controller
    {
        private readonly InternetBankingContext _context;

        public LoginController(InternetBankingContext context) => _context = context;

        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var login = await _context.Logins.SingleOrDefaultAsync(x => x.LoginID == loginID);

            // Login failed
            if (login == null || !PBKDF2.Verify(login.PasswordHash, password))
            { 
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // Login blocked
            else if (login.IsBlocked)
            {
                ModelState.AddModelError("LoginBlocked", "Login account is blocked.");
                return View(new Login { LoginID = loginID });
            }

            // Login succeeded
            else
            {
                // Create sessions for customer info
                HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
                HttpContext.Session.SetString(nameof(Customer.CustomerName), login.Customer.CustomerName);

                return RedirectToAction("Index", "Customer");
            }
        }


        [Route("[action]")]
        public IActionResult Logout()
        {
            // Remove session
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
