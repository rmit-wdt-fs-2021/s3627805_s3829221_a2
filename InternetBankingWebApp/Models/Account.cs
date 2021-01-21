using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBankingWebApp.Models
{

    public enum AccountType
    {
        Saving = 'S',
        Checking = 'C'
    }


    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [Display(Name = "Type")]
        public AccountType AccountType { get; set; }

        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<BillPay> BillPays { get; set; }


        public decimal GetBalance()
        {
            decimal balance = 0;
            foreach (var x in Transactions)
            {
                if (x.TransactionType == TransactionType.Deposit 
                    || x.TransactionType == TransactionType.Transfer && x.DestAccount == null)
                    balance += x.Amount;
                else
                    balance -= x.Amount;
            }

            return balance;
        }


        // Check if this account has free transaction left
        public bool HasServiceFee()
        {
            var count = Transactions.Count(x => x.TransactionType == TransactionType.Withdrawal 
                                                || x.TransactionType == TransactionType.Transfer
                                                && x.DestAccount != null);

            if (count < 4)
                return false;
            else
                return true;
        }


        public void Deposit(decimal amount, string comment)
        {
            Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = AccountNumber,
                    Account = this,
                    Amount = amount,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });
        }


        public void Withdraw(decimal amount, string comment)
        {
            Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdrawal,
                    AccountNumber = AccountNumber,
                    Account = this,
                    Amount = amount,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            if (HasServiceFee())
            {
                decimal serviceFee = 0.1m;
                ServiceCharge(serviceFee, "Service charge on withdrawal");
            }
        }


        public void Transfer(decimal amount, Account destAccount, string comment)
        {
            // Debit this account
            Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    AccountNumber = AccountNumber,
                    Account = this,
                    DestAccountNumber = destAccount.AccountNumber,
                    DestAccount = destAccount,
                    Amount = amount,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            if (HasServiceFee())
            {
                decimal serviceFee = 0.2m;
                ServiceCharge(serviceFee, "Service charge on transfer");
            }

            // Credit destination account
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    AccountNumber = destAccount.AccountNumber,
                    Account = destAccount,
                    Amount = amount,
                    ModifyDate = DateTime.UtcNow
                });
        }


        public void ServiceCharge(decimal amount, string comment)
        {
            Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    AccountNumber = AccountNumber,
                    Account = this,
                    Amount = amount,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });
        }


        public void BillPay()
        {

        }
    }
}
