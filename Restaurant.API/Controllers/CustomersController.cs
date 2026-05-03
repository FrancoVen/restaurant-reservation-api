using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Errors;
using Restaurant.Application.Dtos.Customers;
using Restaurant.Application.Services.Customers.Interfaces;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Manages restaurant customers.
    /// </summary>
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns>List of all customers.</returns>
        /// <response code="200">Returns the list of customers.</response>
        /// <response code="401">User is not authenticated.</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CustomerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyCollection<CustomerDTO>>> Get()
        {
            var result = await _customerService.GetAllAsync();
            return result.Match<ActionResult<IReadOnlyCollection<CustomerDTO>>>(success => Ok(success), errors => this.ToActionResult<IReadOnlyCollection<CustomerDTO>>(errors));
        }


        /// <summary>
        /// Retrieves a customer by ID including their reservations.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <returns>The customer matching the given ID.</returns>
        /// <response code="200">Returns the customer.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Customer not found.</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(CustomerReservationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerReservationDTO>> Get(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return result.Match<ActionResult<CustomerReservationDTO>>(value => Ok(value), errors => this.ToActionResult<CustomerReservationDTO>(errors));
        }

        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <param name="customerCreationDTO">Customer details.</param>
        /// <returns>The created customer.</returns>
        /// <response code="201">Customer created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="409">A customer with the same email already exists.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CustomerDTO>> Post(CustomerCreationDTO customerCreationDTO)
        {
            var result = await _customerService.CreateAsync(customerCreationDTO);
            return result.Match<ActionResult<CustomerDTO>>(value => CreatedAtAction(nameof(Get), new { id = value.Id }, value), errors => this.ToActionResult<CustomerDTO>(errors));
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="id">The customer ID to update.</param>
        /// <param name="customerCreationDTO">Updated customer details.</param>
        /// <response code="204">Customer updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Customer not found.</response>
        /// <response code="409">Email address is already in use.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, CustomerCreationDTO customerCreationDTO)
        {
            var result = await _customerService.UpdateAsync(id, customerCreationDTO);
            return result.Match<IActionResult>(Success => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        /// <param name="id">The customer ID to delete.</param>
        /// <response code="204">Customer deleted successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Customer not found.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            return result.Match<IActionResult>(Success => NoContent(), errors => this.ToActionResult(errors));
        }
    }
}