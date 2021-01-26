using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBankingWebApp.Models
{
    public enum TransactionType
    {
        Deposit = 'D',
        Withdrawal = 'W',
        Transfer = 'T',
        ServiceCharge = 'S',
        BillPay = 'B'
    }


    public record Transaction : IComparable<Transaction>
    {   
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; init; }

        [Display(Name = "Type")]
        public TransactionType TransactionType { get; init; }

        [ForeignKey("Account")]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; init; }
        public virtual Account Account { get; init; }

        [ForeignKey("DestAccount")]
        [Display(Name = "Destination Account")]
        public int? DestAccountNumber { get; init; }
        public virtual Account DestAccount { get; init; }

        [DataType(DataType.Currency), Column(TypeName = "money")]
        public decimal Amount { get; init; }

        [StringLength(255)]
        public string Comment { get; init; }

        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; init; }


        // Sort transactions by time in descending order
        public int CompareTo(Transaction transaction)
        {
            return transaction.ModifyDate.CompareTo(ModifyDate);
        }
    }
}
