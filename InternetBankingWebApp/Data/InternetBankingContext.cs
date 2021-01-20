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


        public InternetBankingContext(DbContextOptions<InternetBankingContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Validations using Fluent API
            builder.Entity<Customer>();
        }

    }
}
