using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBankingWebApp.Models
{

    public class Payee
    {
        [Display(Name = "Payee ID")]
        public int PayeeID { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Payee Name")]
        public string PayeeName { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        public State? State { get; set; }

        [StringLength(4, MinimumLength = 4)]
        public string PostCode { get; set; }

        [Required, StringLength(15), RegularExpression(@"^\+61 [0-9]{4} [0-9]{4}$")]
        public string Phone { get; set; }

        public virtual List<BillPay> BillPays { get; set; }
    }
}
