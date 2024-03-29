﻿using InternetBankingWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using InternetBankingWebApp.Utilities;
using InternetBankingWebApp.Models;

namespace InternetBankingWebApp.BackgroundServices
{
    public class BillPayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BillPayBackgroundService> _logger;


        public BillPayBackgroundService(IServiceProvider services, ILogger<BillPayBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Execute automatic bill pay when no shutdown request is received by web server
            while (!cancellationToken.IsCancellationRequested)
            {
                await PayBill(cancellationToken);

                _logger.LogInformation("BillPay background service is waiting 10 seconds.");

                // Check the bill pay every 10 seconds
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }


        private async Task PayBill(CancellationToken cancellationToken)
        {
            _logger.LogInformation("People Background Service is working.");

            // Inject the context class
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InternetBankingContext>();

            // Process all BillPays
            var billPays = await context.BillPays.ToListAsync(cancellationToken);
            foreach (var billPay in billPays)
            {
                // Skip blocked bill schedule
                if (billPay.IsBlocked)
                {
                    _logger.LogInformation("The bill schedule {0} is blocked.", billPay.BillPayID);

                    continue;
                }

                // Execute payment when the schedule time comes
                if (billPay.ScheduleDate.CompareTo(DateTime.UtcNow) <= 0)
                {
                    // Validation for minimum balance
                    try
                    {
                        billPay.Account.PayBill(billPay.Amount, billPay.Payee);
                    }
                    catch (MinBalanceBreachException)
                    {
                        billPay.IsFailed = true;
                        context.Update(billPay);
                        await context.SaveChangesAsync(cancellationToken);

                        _logger.LogInformation("The bill payment from {0} to {1} is failed due to insufficient fund.",
                            billPay.AccountNumber, billPay.Payee.PayeeName);

                        continue;
                    }

                    _logger.LogInformation("The bill payment from {0} to {1} is complete.",
                        billPay.AccountNumber, billPay.Payee.PayeeName);

                    // Modify the schedule date
                    if (billPay.Period == Period.OnceOff)
                    {
                        context.Remove(billPay);
                        _logger.LogInformation("The bill schedule of {0} is closed.", billPay.BillPayID);
                    }
                    else if (billPay.Period == Period.Monthly)
                    {
                        billPay.ScheduleDate = billPay.ScheduleDate.AddMonths(1);
                        context.Update(billPay);
                        _logger.LogInformation("The next schedule of bill {0} is one month later.", billPay.BillPayID);
                    }
                    else if (billPay.Period == Period.Quarterly)
                    {
                        billPay.ScheduleDate = billPay.ScheduleDate.AddMonths(3);
                        context.Update(billPay);
                        _logger.LogInformation("The next schedule of bill {0} is three months later.", billPay.BillPayID);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
