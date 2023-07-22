using CustomerApiTest.Mediators.Requests;
using FluentValidation;

namespace CustomerApiTest.Validators
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(customer => customer.CustomerCode).NotEmpty().WithMessage("customerCode tidak boleh kosong");
            RuleFor(customer => customer.CustomerName).NotEmpty().WithMessage("customerName tidak boleh kosong");
            RuleFor(customer => customer.CreatedBy).NotEmpty().WithMessage("customer createdBy tidak boleh kosong");
        }
    }

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(customer => customer.CustomerId).NotEmpty().WithMessage("customerId tidak boleh kosong")
                   .GreaterThan(0).WithMessage("customerId harus lebih dari 0");
            RuleFor(customer => customer.CustomerCode).NotEmpty().WithMessage("customerCode tidak boleh kosong");
            RuleFor(customer => customer.CustomerName).NotEmpty().WithMessage("customerName tidak boleh kosong");
        }
    }
    
}