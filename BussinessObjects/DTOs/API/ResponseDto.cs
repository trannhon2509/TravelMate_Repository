using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.DTOs.API
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; } // Trạng thái thành công
        public T Data { get; set; } // Dữ liệu trả về
        public string Message { get; set; } // Thông báo (nếu có)
        public string ErrorCode { get; set; } // Mã lỗi (nếu có lỗi)
    }
}
