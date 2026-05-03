using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Errors;
using Restaurant.Application.Dtos.Reservations;
using Restaurant.Application.Services.Reservations.Interfaces;



namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Manages restaurant reservations
    /// </summary>
    [ApiController]
    [Route("api/reservations")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            this._reservationService = reservationService;
        }


        /// <summary>
        /// Retrieves all reservations
        /// </summary>
        /// <returns>List of all reservations</returns>
        /// <response code="200">Returns the list of reservations.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission.</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyCollection<ReservationDTO>>> Get()
        {
            var result = await _reservationService.GetAllAsync();

            return result.Match<ActionResult<IReadOnlyCollection<ReservationDTO>>>(Succes => Ok(Succes), Errors => this.ToActionResult<IReadOnlyCollection<ReservationDTO>>(Errors));
        }

        /// <summary>
        /// Retrieves a reservation by ID
        /// </summary>
        /// <param name="id">The reservation ID</param>
        /// <returns>The reservation matching the given ID</returns>
        /// <response code="200">Returns the reservation.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission.</response>
        /// <response code="404">Reservation not found.</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(ReservationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservationDTO>> Get(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);

            return result.Match<ActionResult<ReservationDTO>>(Success => Ok(Success), Errors => this.ToActionResult<ReservationDTO>(Errors));
        }

        /// <summary>
        /// Creates a new reservation
        /// </summary>
        /// <param name="reservationCreationDto">Reservation details</param>
        /// <returns>The created reservation</returns>
        /// <response code="201">Reservation created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have permission.</response>
        /// <response code="404">Customer or table not found.</response>
        /// <response code="409">Time slot is not available.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(ReservationDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReservationDTO>> Post(ReservationCreationDTO reservationCreationDto)
        {
            var result = await _reservationService.CreateAsync(reservationCreationDto);

            return result.Match<ActionResult<ReservationDTO>>(Success => CreatedAtAction(nameof(Get), new { id = Success.Id }, Success), Errors => this.ToActionResult<ReservationDTO>(Errors));
        }


        /// <summary>
        /// Updates an existing reservation 
        /// </summary>
        /// <param name="id">The reservation ID to update</param>
        /// <param name="reservationCreationDTO">Updated reservation details</param>
        /// <returns></returns>
        /// <response code="204">Reservation updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission.</response>
        /// <response code="404">Reservation, customer or table not found.</response>
        /// <response code="409">Time slot is not available.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, ReservationCreationDTO reservationCreationDTO)
        {
            var result = await _reservationService.UpdateAsync(id, reservationCreationDTO);

            return result.Match<IActionResult>(Success => NoContent(), Errors => this.ToActionResult(Errors));
        }


        /// <summary>
        /// Deletes a reservation by ID.
        /// </summary>
        /// <param name="id">The reservation ID to delete.</param>
        /// <response code="204">Reservation deleted successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have permission.</response>
        /// <response code="404">Reservation not found.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteAsync(id);

            return result.Match<IActionResult>(Success => NoContent(), Errors => this.ToActionResult(Errors));
        }

    }
}
