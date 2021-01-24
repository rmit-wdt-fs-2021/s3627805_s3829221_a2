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


        public void CreatePagedList(int initialPage, int pageSize)
        {
            PagedList = (PagedList<Transaction>)Account.Transactions.ToPagedList(initialPage, pageSize);
        }
    }
}
