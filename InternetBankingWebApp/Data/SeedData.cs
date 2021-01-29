using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using InternetBankingWebApp.Models;

namespace InternetBankingWebApp.Data
{
    public class SeedData
    {

        public static void Initialise(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<InternetBankingContext>();

            // Skip database seeding if any customer exists in database
            if (context.Customers.Any())
                return;

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    CustomerName = "Matthew Bolger",
                    TFN = "30607588482",
                    Address = "123 Fake Street",
                    City = "Melbourne",
                    State = State.VIC,
                    PostCode = "3000",
                    Phone = "+61 8854 0754"
                },
                new Customer
                {
                    CustomerID = 2200,
                    CustomerName = "Rodney Cocker",
                    TFN = "93657265583",
                    Address = "456 Real Road",
                    City = "Melbourne",
                    State = State.VIC,
                    PostCode = "3005",
                    Phone = "+61 2038 1474"
                },
                new Customer
                {
                    CustomerID = 2300,
                    CustomerName = "Shekhar Kalra",
                    Phone = "+61 1037 5398"
                });

            const string format = "dd/MM/yyyy hh:mm:ss tt";

            context.Logins.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2",
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Login
                {
                    LoginID = "38074569",
                    CustomerID = 2200,
                    PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04",
                    ModifyDate = DateTime.ParseExact("08/06/2020 09:00:00 PM", format, null)
                },
                new Login
                {
                    LoginID = "17963428",
                    CustomerID = 2300,
                    PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE",
                    ModifyDate = DateTime.ParseExact("08/06/2020 10:00:00 PM", format, null)
                });

            context.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = AccountType.Saving,
                    CustomerID = 2100,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = AccountType.Checking,
                    CustomerID = 2100,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = AccountType.Saving,
                    CustomerID = 2200,
                    ModifyDate = DateTime.ParseExact("08/06/2020 09:00:00 PM", format, null)
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = AccountType.Checking,
                    CustomerID = 2300,
                    ModifyDate = DateTime.ParseExact("08/06/2020 10:00:00 PM", format, null)
                });

            context.Payees.AddRange(
                new Payee
                {
                    PayeeName = "Electricity Co.",
                    Phone = "+61 5792 1900"
                },
                new Payee
                {
                    PayeeName = "Water Co.",
                    Phone = "+61 1889 3502"
                },
                new Payee
                {
                    PayeeName = "Gas Co.",
                    Phone = "+61 7826 4996"
                },
                new Payee
                {
                    PayeeName = "Internet Ltd.",
                    Phone = "+61 7692 6313"
                });

            const string initialDeposit = "Initial deposit";

            context.Transactions.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 10:00:00 PM", format, null)
                });

            context.SaveChanges();
        }
    }
}
