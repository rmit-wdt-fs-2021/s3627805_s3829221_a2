using System;
using System.Collections.Generic;
using System.Linq;
using InternetBankingWebApp.Models;

namespace InternetBankingWebApp.ViewModels
{
    public class EditScheduleViewModel
    {
        public List<Account> Accounts { get; set; }
        public List <Payee> Payees { get; set; }
        public BillPay BillPay { get; set; }
    }
}
