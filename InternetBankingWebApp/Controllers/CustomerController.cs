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
using X.PagedList;
using InternetBankingWebApp.ViewModels;
using Newtonsoft.Json;

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


        [HttpPost, Route("[action]")]
        public async Task<IActionResult> ATMAction(TransactionType transactionType, int accountNumber, int destAccountNumber, decimal amount, string comment)
        {
            if (transactionType == TransactionType.Deposit)
                return await Deposit(accountNumber, amount, comment);
            else if (transactionType == TransactionType.Withdrawal)
                return await Withdraw(accountNumber, amount, comment);
            else
                return await Transfer(accountNumber, destAccountNumber, amount, comment);
        }


        public async Task<IActionResult> Deposit(int accountNumber, decimal amount, string comment)
        {
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ATM));
            }
            else
            {
                account.Deposit(amount, comment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> Withdraw(int accountNumber, decimal amount, string comment)
        {
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            try
            {
                account.Withdraw(amount, comment);
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
                return RedirectToAction(nameof(ATM));
            }
            else
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


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
                return RedirectToAction(nameof(ATM));
            }
            else
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }


        [Route("[action]")]
        public async Task<IActionResult> SelectStatementAccount()
        {
            var accounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();

            return View(accounts);
        }


        [HttpPost, Route("[action]")]
        public async Task<IActionResult> AccountToMyStatement(int accountNumber)
        {
            Account account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);

            HttpContext.Session.SetInt32("AccountNumber", accountNumber);

            return RedirectToAction(nameof(MyStatement));
        }


        [Route("[action]")]
        public async Task<IActionResult> MyStatement(int? page = 1)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountNumber");
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber);
            var myStatement = new MyStatementViewModel(account);
            await myStatement.CreatePagedList(page, 4);

            var accounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();
            ViewData["Accounts"] = accounts;
            

            return View(myStatement);
        }


        [Route("[action]")]
        public async Task<IActionResult> SelectBillPayAccount()
        {
            var accounts = await _context.Accounts.Where(x => x.CustomerID == _customerID).ToListAsync();

            return View(accounts);
        }


        [HttpPost, Route("[action]")]
        public IActionResult GetBillPayAccount(int accountID)
        {
            HttpContext.Session.SetInt32("AccountID", accountID);

            return RedirectToAction(nameof(DisplayBillPays));
        }



        [Route("BillPayList")]
        public async Task<IActionResult> DisplayBillPays()
        {
            var accountID = HttpContext.Session.GetInt32("AccountID");
            var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountID);

            return View(account.BillPays);
        }


        [Route("[action]")]
        public async Task<IActionResult> BillPay()
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountID");

            var billPayViewModel = new BillPayViewModel
            {
                Account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountNumber),
                Payees = await _context.Payees.ToListAsync()
            };

            ViewData["BillPayViewModel"] = billPayViewModel;

            return View();
        }


        [HttpPost, Route("[action]")]
        public async Task<IActionResult> ScheduleBillPay(int payeeID, decimal amount, string scheduleString, Period period)
        {
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(BillPay));
            }
            else
            {
                var accountID = HttpContext.Session.GetInt32("AccountID");
                var account = await _context.Accounts.SingleAsync(x => x.AccountNumber == accountID);

                var payee = await _context.Payees.SingleAsync(x => x.PayeeID == payeeID);

                var schedule = DateTime.ParseExact(scheduleString, "MM/dd/yyyy h:mm tt", null).ToUniversalTime();

                account.ScheduleBillPay(payee, amount, schedule, period);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(DisplayBillPays));
            }
        }


        [Route("[action]")]
        public async Task<IActionResult> EditBillPay(int billPayID)
        {
            var billPay = await _context.BillPays.SingleAsync(x => x.BillPayID == billPayID);

            return View(billPay);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBillPay([Bind("AccountNumber, PayeeID, Amount, ScheduleDate, Period, ModifyDate")] BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billPay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction(nameof(DisplayBillPays));
            }
            else
            {
                return View(billPay);
            }
        }


        [Route("[action]")]
        public IActionResult Privacy() => View();
    }
}
