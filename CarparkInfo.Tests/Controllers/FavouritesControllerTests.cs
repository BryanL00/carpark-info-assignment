using CarparkInfo.Api.Controllers;
using CarparkInfo.Api.Dtos;
using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarparkInfo.Tests.Controllers;

public class FavouritesControllerTests
{
    private readonly Mock<IUserFavouriteRepository> _mockFavRepo = new();
    private readonly Mock<ICarParkRepository> _mockCarParkRepo = new();
    private readonly Mock<IUserRepository> _mockUserRepo = new();
    private readonly FavouritesController _controller;

    private const int UserId = 1;
    private const string CarParkNo = "ACB";

    public FavouritesControllerTests()
    {
        _controller = new FavouritesController(
            _mockFavRepo.Object,
            _mockCarParkRepo.Object,
            _mockUserRepo.Object);
    }

    [Fact]
    public async Task GetFavourites_ReturnsUserFavourites()
    {
        var carPark = MakeCarPark(CarParkNo);
        _mockFavRepo.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(new[] { new UserFavourite { UserId = UserId, CarParkNo = CarParkNo, CreatedAt = DateTime.UtcNow, CarPark = carPark } });

        var result = await _controller.GetFavourites(UserId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<UserFavouriteDto>>(ok.Value);
        Assert.Single(items);
        Assert.Equal(CarParkNo, items.First().CarParkNo);
    }

    [Fact]
    public async Task AddFavourite_ValidRequest_Returns201WithDto()
    {
        var carPark = MakeCarPark(CarParkNo);
        _mockCarParkRepo.Setup(r => r.GetByIdAsync(CarParkNo)).ReturnsAsync(carPark);
        _mockFavRepo.Setup(r => r.ExistsAsync(UserId, CarParkNo)).ReturnsAsync(false);
        _mockUserRepo.Setup(r => r.GetOrCreateAsync(UserId)).ReturnsAsync(new User { Id = UserId, Username = "1" });
        _mockFavRepo.Setup(r => r.AddAsync(It.IsAny<UserFavourite>())).Returns(Task.CompletedTask);

        var result = await _controller.AddFavourite(UserId, new AddFavouriteRequest { CarParkNo = CarParkNo });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        var dto = Assert.IsType<UserFavouriteDto>(created.Value);
        Assert.Equal(CarParkNo, dto.CarParkNo);
    }

    [Fact]
    public async Task AddFavourite_CarParkNotFound_Returns404()
    {
        _mockCarParkRepo.Setup(r => r.GetByIdAsync(CarParkNo)).ReturnsAsync((CarPark?)null);

        var result = await _controller.AddFavourite(UserId, new AddFavouriteRequest { CarParkNo = CarParkNo });

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
    }

    [Fact]
    public async Task AddFavourite_AlreadyFavourited_Returns409Conflict()
    {
        _mockCarParkRepo.Setup(r => r.GetByIdAsync(CarParkNo)).ReturnsAsync(MakeCarPark(CarParkNo));
        _mockFavRepo.Setup(r => r.ExistsAsync(UserId, CarParkNo)).ReturnsAsync(true);

        var result = await _controller.AddFavourite(UserId, new AddFavouriteRequest { CarParkNo = CarParkNo });

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
    }

    [Fact]
    public async Task RemoveFavourite_Exists_Returns204NoContent()
    {
        _mockFavRepo.Setup(r => r.ExistsAsync(UserId, CarParkNo)).ReturnsAsync(true);
        _mockFavRepo.Setup(r => r.RemoveAsync(UserId, CarParkNo)).Returns(Task.CompletedTask);

        var result = await _controller.RemoveFavourite(UserId, CarParkNo);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveFavourite_NotExists_Returns404()
    {
        _mockFavRepo.Setup(r => r.ExistsAsync(UserId, CarParkNo)).ReturnsAsync(false);

        var result = await _controller.RemoveFavourite(UserId, CarParkNo);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
    }

    private static CarPark MakeCarPark(string no) => new()
    {
        CarParkNo = no,
        Address = $"Address for {no}",
        FreeParking = "NO",
        NightParking = false,
        GantryHeight = 2.0m,
        CarParkType = new CarParkType { Id = 1, Name = "SURFACE CAR PARK" },
        ParkingSystem = new ParkingSystem { Id = 1, Name = "ELECTRONIC PARKING" }
    };
}
