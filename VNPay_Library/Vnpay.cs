﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay_Library.Enums;
using VNPay_Library.Models;
using VNPay_Library.Utilities;

namespace VNPay_Library
{
    public class Vnpay : IVnpay
    {
        private string _tmnCode;
        private string _hashSecret;
        private string _callbackUrl;
        private string _baseUrl;
        private string _version;
        private string _orderType;

        public void Initialize(string tmnCode,
            string hashSecret,
            string baseUrl,
            string callbackUrl,
            string version = "2.1.0",
            string orderType = "other")
        {
            _tmnCode = tmnCode;
            _hashSecret = hashSecret;
            _callbackUrl = callbackUrl;
            _baseUrl = baseUrl;
            _version = version;
            _orderType = orderType;

            EnsureParametersBeforePayment();
        }

        public string GetPaymentUrl(PaymentRequest request)
        {
            EnsureParametersBeforePayment();

            if (request.Money < 5000 || request.Money > 1000000000)
            {
                throw new ArgumentException("Số tiền thanh toán phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).");
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                throw new ArgumentException("Không được để trống mô tả giao dịch.");
            }

            if (string.IsNullOrEmpty(request.IpAddress))
            {
                throw new ArgumentException("Không được để trống địa chỉ IP.");
            }

            var helper = new PaymentHelper();
            helper.AddRequestData("vnp_Version", _version);
            helper.AddRequestData("vnp_Command", "pay");
            helper.AddRequestData("vnp_TmnCode", _tmnCode);
            helper.AddRequestData("vnp_Amount", (request.Money * 100).ToString());
            helper.AddRequestData("vnp_CreateDate", request.CreatedDate.ToString("yyyyMMddHHmmss"));
            helper.AddRequestData("vnp_CurrCode", request.Currency.ToString().ToUpper());
            helper.AddRequestData("vnp_IpAddr", request.IpAddress);
            helper.AddRequestData("vnp_Locale", EnumHelper.GetDescription(request.Language));
            helper.AddRequestData("vnp_BankCode", request.BankCode == BankCode.ANY ? string.Empty : request.BankCode.ToString());
            helper.AddRequestData("vnp_OrderInfo", request.Description.Trim());
            helper.AddRequestData("vnp_OrderType", _orderType);
            helper.AddRequestData("vnp_ReturnUrl", _callbackUrl);
            helper.AddRequestData("vnp_TxnRef", request.PaymentId.ToString());

            return helper.GetPaymentUrl(_baseUrl, _hashSecret);
        }

        public PaymentResult GetPaymentResult(IQueryCollection parameters)
        {
            var responseData = parameters
                .Where(kv => !string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_"))
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var vnp_BankCode = responseData.GetValueOrDefault("vnp_BankCode");
            var vnp_BankTranNo = responseData.GetValueOrDefault("vnp_BankTranNo");
            var vnp_CardType = responseData.GetValueOrDefault("vnp_CardType");
            var vnp_PayDate = responseData.GetValueOrDefault("vnp_PayDate");
            var vnp_OrderInfo = responseData.GetValueOrDefault("vnp_OrderInfo");
            var vnp_TransactionNo = responseData.GetValueOrDefault("vnp_TransactionNo");
            var vnp_ResponseCode = responseData.GetValueOrDefault("vnp_ResponseCode");
            var vnp_TransactionStatus = responseData.GetValueOrDefault("vnp_TransactionStatus");
            var vnp_TxnRef = responseData.GetValueOrDefault("vnp_TxnRef");
            var vnp_SecureHash = responseData.GetValueOrDefault("vnp_SecureHash");

            if (string.IsNullOrEmpty(vnp_BankCode)
                || string.IsNullOrEmpty(vnp_OrderInfo)
                || string.IsNullOrEmpty(vnp_TransactionNo)
                || string.IsNullOrEmpty(vnp_ResponseCode)
                || string.IsNullOrEmpty(vnp_TransactionStatus)
                || string.IsNullOrEmpty(vnp_TxnRef)
                || string.IsNullOrEmpty(vnp_SecureHash))
            {
                throw new ArgumentException("Không đủ dữ liệu để xác thực giao dịch");
            }

            var helper = new PaymentHelper();
            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHash"))
                {
                    helper.AddResponseData(key, value);
                }
            }

            var responseCode = (ResponseCode)sbyte.Parse(vnp_ResponseCode);
            var transactionStatusCode = (TransactionStatusCode)sbyte.Parse(vnp_TransactionStatus);

            return new PaymentResult
            {
                PaymentId = long.Parse(vnp_TxnRef),
                VnpayTransactionId = long.Parse(vnp_TransactionNo),
                IsSuccess = transactionStatusCode == TransactionStatusCode.Code_00 && responseCode == ResponseCode.Code_00 && helper.IsSignatureCorrect(vnp_SecureHash, _hashSecret),
                Description = vnp_OrderInfo,
                PaymentMethod = string.IsNullOrEmpty(vnp_CardType) ? "Không xác định" : vnp_CardType,
                Timestamp = string.IsNullOrEmpty(vnp_PayDate)
                    ? DateTime.Now
                    : DateTime.ParseExact(vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                TransactionStatus = new TransactionStatus
                {
                    Code = transactionStatusCode,
                    Description = EnumHelper.GetDescription(transactionStatusCode)
                },
                PaymentResponse = new PaymentResponse
                {
                    Code = responseCode,
                    Description = EnumHelper.GetDescription(responseCode)
                },
                BankingInfor = new BankingInfor
                {
                    BankCode = vnp_BankCode,
                    BankTransactionId = string.IsNullOrEmpty(vnp_BankTranNo) ? "Không xác định" : vnp_BankTranNo,
                }
            };
        }

        private void EnsureParametersBeforePayment()
        {
            if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_tmnCode) || string.IsNullOrEmpty(_hashSecret) || string.IsNullOrEmpty(_callbackUrl))
            {
                throw new ArgumentException("Không tìm thấy BaseUrl, TmnCode, HashSecret, hoặc CallbackUrl");
            }
        }

        public string RefundPayment(long paymentId, double refundAmount, string description)
        {
            EnsureParametersBeforePayment();

            if (refundAmount <= 0)
            {
                throw new ArgumentException("Số tiền hoàn phải lớn hơn 0.");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Mô tả không được để trống.");
            }

            var helper = new PaymentHelper();
            helper.AddRequestData("vnp_Version", _version);
            helper.AddRequestData("vnp_Command", "refund");
            helper.AddRequestData("vnp_TmnCode", _tmnCode);
            helper.AddRequestData("vnp_TxnRef", paymentId.ToString());
            helper.AddRequestData("vnp_Amount", (refundAmount * 100).ToString()); // Số tiền hoàn
            helper.AddRequestData("vnp_OrderInfo", description.Trim());
            helper.AddRequestData("vnp_TransactionDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            helper.AddRequestData("vnp_CreateBy", "System"); // Hoặc lấy từ người thực hiện

            return helper.GetPaymentUrl(_baseUrl, _hashSecret);
        }

    }
}
