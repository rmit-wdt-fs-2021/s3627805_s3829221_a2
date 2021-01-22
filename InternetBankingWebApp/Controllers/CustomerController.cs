using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InternetBankingWebApp.Data;
using InternetBankingWebApp.Models;
using InternetBankingWebApp.Utilities;
using InternetBankingWebApp.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InternetBankingWebApp.Controllers
{
    [AuthorizeCustomer, Route("MyBank")]
    public class CustomerController : Controller
    {
        private readonly InternetBankingContext _context;

        // Get session of customer ID from login page
        private int _customerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;


        public CustomerController(InternetBankingContext context) => _context = context;


        [Route("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.Include(x => x.Accounts).SingleAsync(x => x.CustomerID == _customerID);


            return View(customer);
        }


        [Route("[action]")]
        public async Task<IActionResult> ATM()
        {
            var accounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();

            return View(accounts);
        }


        [HttpPost]
        public async Task<IActionResult> Deposit(int accountNumber, decimal amount, string comment)
        {
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            if (!ModelState.IsValid)
            {
                var accounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();
                return View(accounts);
            }

            account.Deposit(amount, comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Route("[action]")]
        public IActionResult Privacy() => View();
    }
}
