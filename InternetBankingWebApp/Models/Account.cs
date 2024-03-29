﻿using System;
using System.Collections.Generic;
using System.Linq;
using InternetBankingWebApp.Utilities;
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
            decimal serviceFee = HasServiceFee() ? 0.1m : 0;

            // If minimum balance is breached
            if (AccountType == AccountType.Saving && GetBalance() - amount - serviceFee < 0)
            {
                throw new MinBalanceBreachException();
            }
            else if (AccountType == AccountType.Checking && GetBalance() - amount - serviceFee < 200)
            {
                throw new MinBalanceBreachException();
            }

            // Withdraw
            else
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

                // If service charge applies
                if (serviceFee != 0)
                    ServiceCharge(serviceFee, "Service charge on withdrawal");
            }
        }


        public void Transfer(decimal amount, Account destAccount, string comment)
        {
            decimal serviceFee = HasServiceFee() ? 0.2m : 0;

            // If minimum balance is breached
            if (AccountType == AccountType.Saving && GetBalance() - amount - serviceFee < 0)
            {
                throw new MinBalanceBreachException();
            }
            else if (AccountType == AccountType.Checking && GetBalance() - amount - serviceFee < 200)
            {
                throw new MinBalanceBreachException();
            }

            // Transfer
            else
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

                // If service charge applies
                if (HasServiceFee())
                    ServiceCharge(serviceFee, "Service charge on transfer");

                // Credit destination account
                destAccount.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.Transfer,
                        AccountNumber = destAccount.AccountNumber,
                        Account = destAccount,
                        Amount = amount,
                        Comment = comment,
                        ModifyDate = DateTime.UtcNow
                    });
            }
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


        public void ScheduleBillPay(Payee payee, decimal amount, DateTime scheduleDate, Period period)
        {
            BillPays.Add(
                new BillPay
                {
                    AccountNumber = AccountNumber,
                    Account = this,
                    PayeeID = payee.PayeeID,
                    Payee = payee,
                    Amount = amount,
                    ScheduleDate = scheduleDate,
                    Period = period,
                    ModifyDate = DateTime.UtcNow
                });
        }


        public void PayBill(decimal amount, Payee payee)
        {
            // If minimum balance is breached
            if (AccountType == AccountType.Saving && GetBalance() - amount < 0)
            {
                throw new MinBalanceBreachException();
            }
            else if (AccountType == AccountType.Checking && GetBalance() - amount < 200)
            {
                throw new MinBalanceBreachException();
            }
            else
            {
                Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.BillPay,
                        AccountNumber = AccountNumber,
                        Account = this,
                        Amount = amount,
                        Comment = "Bill payment to " + payee.PayeeName,
                        ModifyDate = DateTime.UtcNow
                    }); 
            }
        }
    }
}
