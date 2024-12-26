using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay_Library.Models;

namespace VNPay_Library
{
    public interface IVnpay
    {
        void Initialize(
            string tmnCode,
            string hashSecret,
            string baseUrl,
            string callbackUrl,
            string version = "2.1.0",
            string orderType = "other");

        string GetPaymentUrl(PaymentRequest request);

        PaymentResult GetPaymentResult(IQueryCollection parameters);

        string RefundPayment(long paymentId, double refundAmount, string description);

    }
}
