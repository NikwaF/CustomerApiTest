using CustomerApiTest.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CustomerApiTest.DataAccess.Interfaces;
using CustomerApiTest.DataAccess.Repositories;
using FluentValidation;
using CustomerApiTest.Validators;

namespace CustomerApiTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
            ));

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("CustomerApiTest.Mediators")));
            //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetAllCustomersQuery)));


            //builder.Services.AddValidatorsFromAssemblyContaining<CustomerCommandValidator>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

       
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment())
            // {
            app.UseSwagger();
            app.UseSwaggerUI();
            // }
          
            app.UseAuthorization();

          app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return System.Threading.Tasks.Task.CompletedTask;
            });


            app.MapControllers();

            app.Run();
            
        }
    }
}