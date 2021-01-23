using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetBankingWebApp.Models;
using X.PagedList;

namespace InternetBankingWebApp.ViewModels
{
    public class MyStatementViewModel
    {
        public Account Account { get; set; }
        public decimal Balance { get; set; }
        public PagedList<Transaction> PagedList { get; set; }


        public MyStatementViewModel(Account account)
        {
            Account = account;
            Balance = Account.GetBalance();
        }


        public async Task CreatePagedList(int initialPage, int pageSize)
        {
            PagedList = (PagedList<Transaction>)await Account.Transactions.ToPagedListAsync(initialPage, pageSize);
        }
    }
}
