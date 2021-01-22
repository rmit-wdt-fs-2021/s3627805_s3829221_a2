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
using System.Collections.Generic;

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
            var customerAccounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();
            var allAccounts = await _context.Accounts.ToListAsync();
            var accountList = new List<List<Account>>() {customerAccounts, allAccounts};

            ViewData["AccountList"] = accountList;

            return View();
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
                return View();
            }
            else
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Withdraw(int accountNumber, decimal amount, string comment)
        {
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            try
            {
                account.Deposit(amount, comment);
            }
            catch (MinBalanceBreachException)
            {
                if (account.AccountType == AccountType.Saving)
                    ModelState.AddModelError(nameof(amount), "Saving account must maintain a balance above $0.");
                if (account.AccountType == AccountType.Checking)
                    ModelState.AddModelError(nameof(amount), "Checking account must maintain a balance above $200.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Transfer(int accountNumber, int destAccountNumber ,decimal amount, string comment)
        {
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);
            var destAccount = await _context.Accounts.SingleAsync(x => x.AccountNumber == destAccountNumber);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            try
            {
                account.Transfer(amount, destAccount, comment);
            }
            catch (MinBalanceBreachException)
            {
                if (account.AccountType == AccountType.Saving)
                    ModelState.AddModelError(nameof(amount), "Saving account must maintain a balance above $0.");
                if (account.AccountType == AccountType.Checking)
                    ModelState.AddModelError(nameof(amount), "Checking account must maintain a balance above $200.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


        [Route("[action]")]
        public IActionResult Privacy() => View();
    }
}
