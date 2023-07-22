using MediatR;
using CustomerApiTest.Models;

namespace CustomerApiTest.Mediators.Requests
{
    public class CustomerListResponse
    {
        public IEnumerable<Customer> Customers { get; set; }
    }

    public class CreateCustomerCommand : IRequest<int>
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateCustomerCommand : IRequest
    {
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class DeleteCustomerCommand : IRequest
    {
        public int CustomerId { get; set; }
    }

    public class GetCustomerQuery : IRequest<Customer>
    {
        public int CustomerId { get; set; }
    }

    public class GetAllCustomersQuery : IRequest<CustomerListResponse>
    {
    }
    //GetAllCustomersQuery
}
