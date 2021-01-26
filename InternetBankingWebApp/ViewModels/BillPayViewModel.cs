using System;
using System.Collections.Generic;
using System.Linq;
using InternetBankingWebApp.Models;

namespace InternetBankingWebApp.ViewModels
{
    public class BillPayViewModel
    {
        public List<Account> Accounts { get; set; }
        public List<Payee> Payees { get; set; }
    }
}
