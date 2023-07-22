using CustomerApiTest.DataAccess.Interfaces;
using CustomerApiTest.Mediators.Requests;
using CustomerApiTest.Models;
using CustomerApiTest.Exceptions;
using MediatR;

namespace CustomerApiTest.Mediators.Handlers
{
    public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, CustomerListResponse>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerListResponse> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync();

            var response = new CustomerListResponse
            {
                Customers = customers
            };

            return response;
        }
    }

    public class GetCustomersHandler : IRequestHandler<GetCustomerQuery, Customer>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

            if(customers == null)
            {
                throw new NotFoundException("customer id tidak ditemukan");
            }

            return customers;
        }
    }

    public class UpdateCustomersHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer customer;
            try
            {
                customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

                if(customer == null)
                {
                    throw new NotFoundException($"Customer dengan customerId {request.CustomerId} tidak dapat ditemukan");
                }

                customer.customerName = request.CustomerName;
                customer.customerAddress = request.CustomerAddress;
                customer.customerCode= request.CustomerCode;
                customer.modifiedBy = request.ModifiedBy;
                customer.modifiedAt = DateTime.Now;

                await _customerRepository.UpdateCustomerAsync(customer);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class DeleteCustomersHandler : IRequestHandler<DeleteCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
           
            try
            {
                Customer customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

                if (customer == null)
                {
                    throw new NotFoundException($"Customer dengan customerId {request.CustomerId} tidak dapat ditemukan");
                }

                await _customerRepository.DeleteCustomerAsync(customer);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class InsertCustomersHandler : IRequestHandler<CreateCustomerCommand,int>
    {
        private readonly ICustomerRepository _customerRepository;

        public InsertCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Customer customer = new Customer();
                customer.customerName = request.CustomerName;
                customer.customerAddress = request.CustomerAddress;
                customer.customerCode = request.CustomerCode;
                customer.createdAt = DateTime.Now;
                customer.createdBy = request.CreatedBy;

                Customer newCustomer = await _customerRepository.CreateCustomerAsync(customer);

                return newCustomer.customerId;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
