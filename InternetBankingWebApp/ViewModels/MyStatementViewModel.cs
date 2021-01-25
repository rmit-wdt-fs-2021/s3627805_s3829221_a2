using System.Threading.Tasks;
using InternetBankingWebApp.Models;
using X.PagedList;

namespace InternetBankingWebApp.ViewModels
{
    public class MyStatementViewModel
    {
        public Account Account { get; set; }
        public decimal Balance { get; set; }
        public IPagedList<Transaction> PagedList { get; set; }


        public MyStatementViewModel(Account account)
        {
            Account = account;
            Balance = Account.GetBalance();
        }


        public async Task CreatePagedList(int? page, int pageSize)
        {
            PagedList = await Account.Transactions.ToPagedListAsync((int)page, pageSize);
        }
    }
}
