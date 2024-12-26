using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.DTOs.API
{
    public class PaginatedResponseDto<T>
    {
        public bool Success { get; set; } // Trạng thái thành công
        public IEnumerable<T> Items { get; set; } // Danh sách dữ liệu
        public int PageNumber { get; set; } // Số trang hiện tại
        public int PageSize { get; set; } // Số lượng dữ liệu mỗi trang
        public long TotalItems { get; set; } // Tổng số dữ liệu
        public string Message { get; set; } // Thông báo (nếu có)
    }

}
