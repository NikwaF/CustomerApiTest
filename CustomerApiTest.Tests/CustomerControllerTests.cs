using Castle.Core.Resource;
using CustomerApiTest.Controllers;
using CustomerApiTest.DataAccess.Data;
using CustomerApiTest.DataAccess.Interfaces;
using CustomerApiTest.DataAccess.Repositories;
using CustomerApiTest.Exceptions;
using CustomerApiTest.Mediators.Requests;
using CustomerApiTest.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CustomerApiTest.Tests
{
    public class CustomerControllerTests
    {
        private readonly ICustomerRepository _repository;
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<IMediator> _mockMediator;
        //public virtual IDbSet<Customer> Cafe { get; set; }

        public CustomerControllerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CustomerTestDatabase")
                .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(dbContextOptions);
            _repository = new CustomerRepositoryForTest(_mockDbContext.Object);
            _mockMediator = new Mock<IMediator>();

            var customers = new List<Customer>
            {
                new Customer {customerId = 1, customerName = "niko 1", customerCode="NK1", createdBy = 1, createdAt = DateTime.Now },
                new Customer {customerId = 2, customerName = "niko 2", customerCode="NK2", createdBy = 1 , createdAt = DateTime.Now},
                new Customer {customerId = 3,  customerName = "niko 3", customerCode="NK3", createdBy = 1 ,createdAt = DateTime.Now },
            };

            var mockCustomersDbSet = new Mock<DbSet<Customer>>();
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.AsQueryable().Provider);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.AsQueryable().Expression);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.AsQueryable().ElementType);
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.AsQueryable().GetEnumerator());
            mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Customers).Returns(mockCustomersDbSet.Object);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_Returns_Customer()
        {
            var customer = await _repository.GetCustomerByIdAsync(1);

            Assert.NotNull(customer);
            Assert.Equal(1, customer.customerId);
            Assert.Equal("niko 1", customer.customerName);
        }

        [Fact]
        public async Task CreateCustomer_Returns_OkResult_With_CreatedCustomerId()
        {
        
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateCustomerCommand command, CancellationToken token) =>
                {
                    Func<Task<int>> asyncFunc = async () =>
                    {
                        var test = await _repository.GetAllCustomersAsync();
                        return test.Count();
                    };

                    var createdCustomerId = 4;
                    return createdCustomerId;
                });

            var controller = new CustomerController(_mockMediator.Object);

          

            var command = new CreateCustomerCommand
            {
                CustomerCode = "KKK",
                CustomerName = "KIKIKIKI",
                CustomerAddress = "disana",
                CreatedBy= 1
            };

            var result = await controller.InsertCustomer(command);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            var transactionId = responseObject.TransactionId;

            Assert.Equal("4", transactionId);
        }

        [Fact]
        public async Task CreateCustomer_Returns_OkResult_With_Exception500()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("error bingits"));

            var controller = new CustomerController(_mockMediator.Object);

            var command = new CreateCustomerCommand
            {
                CustomerCode = "KKK",
                CustomerName = "KIKIKIKI",
                CustomerAddress = "disana",
                CreatedBy = 1
            };

            var result = await controller.InsertCustomer(command);

            var okResult = Assert.IsType<ObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            Assert.Equal(500, okResult.StatusCode);
            Assert.Equal("error bingits", responseObject.Message);
        }

        [Fact]
        public async Task CreateCustomer_Returns_ErrorResult_With_UncompleteInput()
        {
            var controller = new CustomerController(_mockMediator.Object);

            var command = new CreateCustomerCommand
            {
                CustomerCode = "KKK",
                CustomerName = "KIKIKIKI",
                CustomerAddress = "disana",
            };

            // Act
            var result = await controller.InsertCustomer(command);

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("not ok", responseObject.Message);
      }

        [Fact]
        public async Task UpdateCustomer_Returns_When_CustomerId_NotFound()
        {
            var command = new UpdateCustomerCommand
            {
                CustomerId = 10,
                CustomerCode = "test",
                CustomerName = "test"
            };

            string msgException = $"Customer dengan customerId {command.CustomerId} tidak dapat ditemukan";

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new NotFoundException(msgException));

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.UpdateCustomer(command);

            var okResult = Assert.IsType<ObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal(msgException, responseObject.Message);

        }

        [Fact]
        public async Task UpdateCustomer_Returns_Ok()
        {
            Customer customer = new Customer
            {
                customerId = 1,
                customerCode = "NIU",
                customerName = "niko update",
                createdBy = 1
            };

            var command = new UpdateCustomerCommand
            {
                CustomerId = customer.customerId,
                CustomerCode = customer.customerCode,
                CustomerName = customer.customerName,
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    var customercek = _mockDbContext.Object.Customers.FirstOrDefault(x=> x.customerId == customer.customerId);
                    customercek.customerCode = customer.customerCode;
                    customercek.customerName  = customer.customerName;
                    customercek.modifiedAt = DateTime.Now;

                    _mockDbContext.Object.SaveChanges();
                });

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.UpdateCustomer(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);
            var updatedCustomer = await _repository.GetCustomerByIdAsync(customer.customerId);

            var test = await _repository.GetAllCustomersAsync();

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(customer.customerId.ToString(), responseObject.TransactionId);
            Assert.Equal(customer.customerName, updatedCustomer.customerName);
            Assert.Equal(customer.customerCode, updatedCustomer.customerCode);
            Assert.NotNull(updatedCustomer.modifiedAt);
        }


        [Fact]
        public async Task DeleteCustomer_Returns_When_CustomerId_NotFound()
        {
            var command = new DeleteCustomerCommand
            {
                CustomerId = 10,
            };

            string msgException = $"Customer dengan customerId {command.CustomerId} tidak dapat ditemukan";

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new NotFoundException(msgException));

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.DeleteCustomer(command);

            var okResult = Assert.IsType<ObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal(msgException, responseObject.Message);

        }

        [Fact]
        public async Task DeleteCustomer_ReturnsOk()
        {
            var command = new DeleteCustomerCommand
            {
                CustomerId = 3
            };

            var customers = await _repository.GetAllCustomersAsync();
            var customersCopy = new List<Customer>(customers);

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                   .Callback<DeleteCustomerCommand, CancellationToken>(async (command, cancellationToken) =>
                   {
                       Customer customer = customersCopy.FirstOrDefault(x => x.customerId == command.CustomerId);

                       customersCopy.Remove(customer);
                   })
                   .Returns(Task.CompletedTask);

            var controller = new CustomerController(_mockMediator.Object);

            Customer getOldCustomer = customersCopy.FirstOrDefault(x => x.customerId == command.CustomerId);

            Assert.NotNull(getOldCustomer);

            var result = await controller.DeleteCustomer(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = Assert.IsType<ApiResponse<Customer>>(okResult.Value);

            getOldCustomer = customersCopy.FirstOrDefault(x => x.customerId == command.CustomerId);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Null(getOldCustomer);
        }
    }

}
