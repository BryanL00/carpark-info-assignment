using CarparkInfo.Api.Dtos;
using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarparkInfo.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/favourites")]
public class FavouritesController : ControllerBase
{
    private readonly IUserFavouriteRepository _favouriteRepo;
    private readonly ICarParkRepository _carParkRepo;
    private readonly IUserRepository _userRepo;

    public FavouritesController(
        IUserFavouriteRepository favouriteRepo,
        ICarParkRepository carParkRepo,
        IUserRepository userRepo)
    {
        _favouriteRepo = favouriteRepo;
        _carParkRepo = carParkRepo;
        _userRepo = userRepo;
    }

    /// <summary>
    /// Returns all carparks saved as favourites by the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserFavouriteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavourites(int userId)
    {
        var favourites = await _favouriteRepo.GetByUserIdAsync(userId);
        return Ok(favourites.Select(UserFavouriteDto.From));
    }

    /// <summary>
    /// Adds a carpark to the specified user's favourites list.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">Request body containing the carpark number to favourite.</param>
    [HttpPost]
    [ProducesResponseType(typeof(UserFavouriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavourite(int userId, [FromBody] AddFavouriteRequest request)
    {
        var carPark = await _carParkRepo.GetByIdAsync(request.CarParkNo);
        if (carPark is null)
            return NotFound($"Carpark '{request.CarParkNo}' not found.");

        if (await _favouriteRepo.ExistsAsync(userId, request.CarParkNo))
            return Conflict($"Carpark '{request.CarParkNo}' is already in your favourites.");

        await _userRepo.GetOrCreateAsync(userId);

        var favourite = new UserFavourite
        {
            UserId = userId,
            CarParkNo = request.CarParkNo,
            CreatedAt = DateTime.UtcNow
        };

        await _favouriteRepo.AddAsync(favourite);

        favourite.CarPark = carPark;
        return CreatedAtAction(
            nameof(GetFavourites),
            new { userId },
            UserFavouriteDto.From(favourite));
    }

    /// <summary>
    /// Removes a carpark from the specified user's favourites list.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="carParkNo">The carpark number to remove.</param>
    [HttpDelete("{carParkNo}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavourite(int userId, string carParkNo)
    {
        if (!await _favouriteRepo.ExistsAsync(userId, carParkNo))
            return NotFound($"Carpark '{carParkNo}' is not in your favourites.");

        await _favouriteRepo.RemoveAsync(userId, carParkNo);
        return NoContent();
    }
}
