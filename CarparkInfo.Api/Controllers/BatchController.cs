using CarparkInfo.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarparkInfo.Api.Controllers;

[ApiController]
[Route("api/batch")]
public class BatchController : ControllerBase
{
    private readonly ICarParkImportService _importService;

    public BatchController(ICarParkImportService importService) => _importService = importService;

    /// <summary>
    /// Imports carpark data from a CSV file. Atomically replaces all existing carpark records.
    /// If any row in the file is invalid, the entire import is rolled back.
    /// </summary>
    /// <param name="file">The CSV file to import (must match the HDB carpark information format).</param>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("A non-empty CSV file is required.");

        try
        {
            await using var stream = file.OpenReadStream();
            await _importService.ImportAsync(stream);
            return Ok(new { message = "Import completed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Import failed. No changes were applied.", detail = ex.Message });
        }
    }
}
