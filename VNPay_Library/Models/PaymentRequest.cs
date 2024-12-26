using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay_Library.Enums;

namespace VNPay_Library.Models
{
    public class PaymentRequest
    {
        public long PaymentId { get; set; }

        public string Description { get; set; }

        public double Money { get; set; }

        public string IpAddress { get; set; }

        public BankCode BankCode { get; set; } = BankCode.ANY;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Currency Currency { get; set; } = Currency.VND;

        public DisplayLanguage Language { get; set; } = DisplayLanguage.Vietnamese;
    }
}
