using CustomerApiTest.DataAccess.Interfaces;
using CustomerApiTest.Models;
using Microsoft.EntityFrameworkCore;


namespace CustomerApiTest.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
    }
}
