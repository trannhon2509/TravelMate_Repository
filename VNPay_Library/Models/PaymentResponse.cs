using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay_Library.Enums;

namespace VNPay_Library.Models
{
    public class PaymentResponse
    {
        public ResponseCode Code { get; set; }

        public string Description { get; set; }
    }
}
