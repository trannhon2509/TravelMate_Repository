using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay_Library.Enums;

namespace VNPay_Library.Models
{
    public class TransactionStatus
    {
        /// <summary>
        /// Mã trạng thái của giao dịch do VNPAY định nghĩa.
        /// </summary>
        public TransactionStatusCode Code { get; set; }

        /// <summary>
        /// Mô tả chi tiết về trạng thái giao dịch.
        /// </summary>
        public string Description { get; set; }
    }
}
