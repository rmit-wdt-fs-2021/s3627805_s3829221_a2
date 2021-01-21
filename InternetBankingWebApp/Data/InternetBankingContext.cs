using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetBankingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetBankingWebApp.Data
{
    public class InternetBankingContext : DbContext
    {

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        public InternetBankingContext(DbContextOptions<InternetBankingContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Validations using Fluent API
            builder.Entity<Login>().HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8").
                HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 64");
            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_Amount", "Amount > 0");
            builder.Entity<BillPay>().HasOne(x => x.Account).WithMany(x => x.BillPays).HasForeignKey(x => x.AccountNumber);
            builder.Entity<BillPay>().HasOne(x => x.Payee).WithMany(x => x.BillPays).HasForeignKey(x => x.PayeeID);
            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0");
            builder.Entity<Transaction>().HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);
        }
    }
}
