using CustomerApiTest.DataAccess.Data;
using CustomerApiTest.DataAccess.Interfaces;
using CustomerApiTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApiTest.DataAccess.Repositories
{
    public class CustomerRepositoryForTest : ICustomerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerRepositoryForTest(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            //return await _dbContext.Customers.FirstOrDefaultAsync(c => c.customerId == customerId);
            return _dbContext.Customers.FirstOrDefault(c => c.customerId== customerId);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return _dbContext.Customers.ToList();
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            _dbContext.Entry(customer).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return customer;
        }

        public async Task DeleteCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Remove(customer);
            _dbContext.SaveChanges();
        }
    }
}
