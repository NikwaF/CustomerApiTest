using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApiTest.Models
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public T Data { get; set; }
        public List<ValidationFailure> Error { get; set; }
    }
}
