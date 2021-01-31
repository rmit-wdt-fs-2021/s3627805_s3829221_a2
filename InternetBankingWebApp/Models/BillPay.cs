using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBankingWebApp.Models
{

    public enum Period
    {
        Monthly = 'M',
        Quarterly = 'Q',
        OnceOff = 'S'
    }


    public class BillPay
    {
        [Display(Name = "Billpay ID")]
        public int BillPayID { get; set; }

        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("Payee")]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Schedule Date")]
        public DateTime ScheduleDate { get; set; }

        [Required]
        public Period Period { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }
        public bool IsBlocked { get; set; } = false;
        public bool IsFailed { get; set; }
    }
}
