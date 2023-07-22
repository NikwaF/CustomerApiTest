using CustomerApiTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApiTest.DataAccess.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }
    }
}
