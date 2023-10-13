using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class ResponseDTO<T>
    {
        public T Data { get; set; }
        public ResponseError Error { get; set; }

    }
}
