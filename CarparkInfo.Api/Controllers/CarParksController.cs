using CarparkInfo.Api.Dtos;
using CarparkInfo.Domain.Models;
using CarparkInfo.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarparkInfo.Api.Controllers;

[ApiController]
[Route("api/carparks")]
public class CarParksController : ControllerBase
{
    private readonly ICarParkRepository _repo;

    public CarParksController(ICarParkRepository repo) => _repo = repo;

    /// <summary>
    /// Returns a list of carparks, optionally filtered by free parking, night parking, and vehicle height.
    /// </summary>
    /// <param name="freeParking">Set to true to return only carparks that offer free parking.</param>
    /// <param name="nightParking">Set to true to return only carparks that allow night parking.</param>
    /// <param name="vehicleHeight">Return only carparks whose gantry height is greater than or equal to this value (in metres).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CarParkDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? freeParking,
        [FromQuery] bool? nightParking,
        [FromQuery] decimal? vehicleHeight)
    {
        var filter = new CarParkFilter
        {
            FreeParking = freeParking,
            NightParking = nightParking,
            VehicleHeight = vehicleHeight
        };

        var carParks = await _repo.GetAllAsync(filter);
        return Ok(carParks.Select(CarParkDto.From));
    }

    /// <summary>
    /// Returns the details of a single carpark by its carpark number.
    /// </summary>
    /// <param name="id">The unique carpark number (e.g. ACB).</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CarParkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var carPark = await _repo.GetByIdAsync(id);
        if (carPark is null) return NotFound();
        return Ok(CarParkDto.From(carPark));
    }
}
