using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Common
{
    public class PaginatedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Data { get; set; }
        public int Total { get; set; }

        public static PaginatedResult<T> GetDefault(int pageIndex, int pageSize)
        {
            return new PaginatedResult<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Data = new List<T>(),
                Total = 0
            };
        }

    }
}
