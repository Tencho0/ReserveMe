using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Enums;
using ReserveMe.Server.Interfaces;

namespace ReserveMe.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet("venue/{venueId:int}")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetTablesByVenue(int venueId, [FromQuery] bool includeInactive = false)
        {
            var tables = await _tableService.GetTablesByVenueAsync(venueId, includeInactive);
            return Ok(tables);
        }

        [HttpGet("{tableId:int}")]
        public async Task<ActionResult<TableDto>> GetTable(int tableId)
        {
            var table = await _tableService.GetTableByIdAsync(tableId);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpGet("venue/{venueId:int}/statistics")]
        public async Task<ActionResult<TableStatisticsDto>> GetStatistics(int venueId)
        {
            var stats = await _tableService.GetTableStatisticsAsync(venueId);
            return Ok(stats);
        }

        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable([FromBody] CreateTableDto dto)
        {
            try
            {
                var table = await _tableService.CreateTableAsync(dto);
                return CreatedAtAction(nameof(GetTable), new { tableId = table.Id }, table);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{tableId:int}")]
        public async Task<ActionResult<TableDto>> UpdateTable(int tableId, [FromBody] UpdateTableDto dto)
        {
            var table = await _tableService.UpdateTableAsync(tableId, dto);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPatch("{tableId:int}/status")]
        public async Task<ActionResult<TableDto>> ChangeStatus(int tableId, [FromBody] ChangeTableStatusDto dto)
        {
            var table = await _tableService.ChangeTableStatusAsync(tableId, dto.Status);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPost("{tableId:int}/release")]
        public async Task<ActionResult<TableDto>> ReleaseTable(int tableId)
        {
            var table = await _tableService.ReleaseTableAsync(tableId);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPost("{tableId:int}/seat")]
        public async Task<ActionResult<TableDto>> SeatReservation(int tableId)
        {
            try
            {
                var table = await _tableService.SeatReservationAsync(tableId);
                if (table == null) return NotFound();
                return Ok(table);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{tableId:int}/toggle-active")]
        public async Task<ActionResult<TableDto>> ToggleActive(int tableId)
        {
            var table = await _tableService.ToggleTableActiveAsync(tableId);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpDelete("{tableId:int}")]
        public async Task<ActionResult> DeleteTable(int tableId)
        {
            var result = await _tableService.DeleteTableAsync(tableId);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}