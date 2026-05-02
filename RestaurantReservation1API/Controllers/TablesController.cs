using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Errors;
using Restaurant.Application.Dtos.Tables;
using Restaurant.Application.Services.Tables.Interfaces;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Manages restaurant tables.
    /// </summary>
    [ApiController]
    [Route("api/tables")]
    [Authorize]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        /// <summary>
        /// Retrieves all tables.
        /// </summary>
        /// <returns>List of all tables.</returns>
        /// <response code="200">Returns the list of tables.</response>
        /// <response code="401">User isn't authenticated.</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(IReadOnlyCollection<TableDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyCollection<TableDTO>>> Get()
        {
            var result = await _tableService.GetAllAsync();
            return result.Match<ActionResult<IReadOnlyCollection<TableDTO>>>(success => Ok(success), errors => this.ToActionResult<IReadOnlyCollection<TableDTO>>(errors));
        }

        /// <summary>
        /// Retrieves a table by ID.
        /// </summary>
        /// <param name="id">The table ID.</param>
        /// <returns>The table matching the given ID.</returns>
        /// <response code="200">Returns the table.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="404">Table not found.</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(TableReservationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TableReservationDTO>> Get(int id)
        {
            var result = await _tableService.GetByIdAsync(id);
            return result.Match<ActionResult<TableReservationDTO>>(value => Ok(value), errors => this.ToActionResult<TableReservationDTO>(errors));
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="tableCreationDTO">Table details.</param>
        /// <returns>The created table.</returns>
        /// <response code="201">Table created successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="409">A table with the same number already exists.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TableDTO>> Post(TableCreationDTO tableCreationDTO)
        {
            var result = await _tableService.CreateAsync(tableCreationDTO);
            return result.Match<ActionResult<TableDTO>>(value => CreatedAtAction(nameof(Get), new { id = value.Id }, value), errors => this.ToActionResult<TableDTO>(errors));
        }

        /// <summary>
        /// Updates an existing table.
        /// </summary>
        /// <param name="id">The table ID to update.</param>
        /// <param name="tableCreationDTO">Updated table details.</param>
        /// <response code="204">Table updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">Table not found.</response>
        /// <response code="409">Table number is already assigned to another table.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, TableCreationDTO tableCreationDTO)
        {
            var result = await _tableService.UpdateAsync(id, tableCreationDTO);
            return result.Match<IActionResult>(Success => NoContent(), errors => this.ToActionResult(errors));
        }

        /// <summary>
        /// Deletes a table by ID.
        /// </summary>
        /// <param name="id">The table ID to delete.</param>
        /// <response code="204">Table deleted successfully.</response>
        /// <response code="401">User isn't authenticated.</response>
        /// <response code="403">User doesn't have permission. Requires Admin role.</response>
        /// <response code="404">Table not found.</response>
        /// <response code="409">Table has active reservations and cannot be deleted.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tableService.DeleteAsync(id);
            return result.Match<IActionResult>(Success => NoContent(), errors => this.ToActionResult(errors));
        }
    }
}