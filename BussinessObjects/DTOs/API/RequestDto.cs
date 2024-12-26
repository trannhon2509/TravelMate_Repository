using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.DTOs.API
{
    public class RequestDto<T>
    {
        public T Data { get; set; } // Dữ liệu yêu cầu
        public string UserToken { get; set; } // Token xác thực của người dùng (nếu cần)
    }
}
