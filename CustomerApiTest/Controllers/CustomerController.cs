using CustomerApiTest.Exceptions;
using CustomerApiTest.Mediators.Requests;
using CustomerApiTest.Models;
using CustomerApiTest.Validators;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;

namespace CustomerApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        //private IValidator<CreateCustomerCommand> _validator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetCustomerData")]
        public async Task<IActionResult> GetAllCustomers()
        {
            ApiResponse<CustomerListResponse> response = new ApiResponse<CustomerListResponse>
            {
                Message = "ok",
                TransactionId = null,
                Data = null
            };

            try
            {
                var data = await _mediator.Send(new GetAllCustomersQuery());
                response.Data= data;
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                return StatusCode(204, response);
            }

            return Ok(response);
        }

        [HttpGet("{id}",Name = "GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>
            {
                Message = "ok",
                TransactionId = null,
                Data = null
            };

            try
            {
                var data = await _mediator.Send(new GetCustomerQuery { CustomerId = id});
                response.Data = data;
            }
            catch (NotFoundException)
            {
                return NoContent();
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        // PUT api/customer/{id}
        [HttpPut(Name ="UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody]UpdateCustomerCommand command)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>
            {
                Message = "ok",
                TransactionId = null,
                Data = null
            };

            UpdateCustomerCommandValidator validator = new UpdateCustomerCommandValidator();
            ValidationResult result = validator.Validate(command);

            if (!result.IsValid)
            {
                List<ValidationFailure> error = result.Errors;
                response.Message = "not ok";
                response.Error = error;
                return BadRequest(response);
            }

            try
            {
                 await _mediator.Send(command);
            }
            catch (NotFoundException e)
            {
                response.Message = e.Message;   
                return StatusCode(400, response);
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                return StatusCode(500, response);
            }

            response.TransactionId = command.CustomerId.ToString();

            return Ok(response);
        }

        // DELETE api/customer/{id}
        [HttpDelete(Name = "DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromBody]DeleteCustomerCommand command)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>
            {
                Message = "ok",
                TransactionId = null,
                Data = null
            };

            try
            {
                await _mediator.Send(command);
            }
            catch (NotFoundException e)
            {
                response.Message = e.Message;
                return StatusCode(400, response);
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                return StatusCode(500, response);
            }

            response.TransactionId = command.CustomerId.ToString();

            return Ok(response);
        }


        // POST api/customer
        [HttpPost(Name ="InsertCustomer")]
        public async Task<IActionResult> InsertCustomer([FromBody] CreateCustomerCommand command)
        {
            CreateCustomerCommandValidator validator = new CreateCustomerCommandValidator();
            ValidationResult result = validator.Validate(command);

            ApiResponse<Customer> response = new ApiResponse<Customer>
            {
                Message = "ok",
                TransactionId = null,
                Data = null
            };

            if (!result.IsValid)
            {
                List<ValidationFailure> error = result.Errors;
                response.Message = "not ok";
                response.Error = error;
                return BadRequest(response);
            }
          

            try
            {
                int createCustomer = await _mediator.Send(command);
                response.TransactionId = createCustomer.ToString();
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }
    }
}
