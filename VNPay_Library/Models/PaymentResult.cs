using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPay_Library.Models
{
    public class PaymentResult
    {
        public long PaymentId { get; set; }

        public bool IsSuccess { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; }

        public long VnpayTransactionId { get; set; }

        public string PaymentMethod { get; set; }

        public PaymentResponse PaymentResponse { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public BankingInfor BankingInfor { get; set; }

    }
}
