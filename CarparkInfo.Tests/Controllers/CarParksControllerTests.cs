using CarparkInfo.Api.Controllers;
using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Models;
using CarparkInfo.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarparkInfo.Tests.Controllers;

public class CarParksControllerTests
{
    private readonly Mock<ICarParkRepository> _mockRepo = new();
    private readonly CarParksController _controller;

    public CarParksControllerTests()
    {
        _controller = new CarParksController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_NoFilter_ReturnsAllCarParks()
    {
        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CarParkFilter>()))
            .ReturnsAsync(new[] { MakeCarPark("ACB"), MakeCarPark("ACM") });

        var result = await _controller.GetAll(null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        var items = Assert.IsAssignableFrom<IEnumerable<Api.Dtos.CarParkDto>>(ok.Value);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task GetAll_WithFreeParkingFilter_PassesCorrectFilterToRepository()
    {
        _mockRepo.Setup(r => r.GetAllAsync(It.Is<CarParkFilter>(f => f.FreeParking == true)))
            .ReturnsAsync(new[] { MakeCarPark("ACM") });

        var result = await _controller.GetAll(freeParking: true, nightParking: null, vehicleHeight: null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<Api.Dtos.CarParkDto>>(ok.Value);
        Assert.Single(items);
        _mockRepo.Verify(r => r.GetAllAsync(It.Is<CarParkFilter>(f =>
            f.FreeParking == true &&
            f.NightParking == null &&
            f.VehicleHeight == null)), Times.Once);
    }

    [Fact]
    public async Task GetAll_WithNightParkingAndHeightFilter_PassesCorrectFilterToRepository()
    {
        _mockRepo.Setup(r => r.GetAllAsync(It.Is<CarParkFilter>(f =>
            f.NightParking == true && f.VehicleHeight == 2.1m)))
            .ReturnsAsync(Array.Empty<CarPark>());

        var result = await _controller.GetAll(freeParking: null, nightParking: true, vehicleHeight: 2.1m);

        var ok = Assert.IsType<OkObjectResult>(result);
        _mockRepo.Verify(r => r.GetAllAsync(It.Is<CarParkFilter>(f =>
            f.NightParking == true && f.VehicleHeight == 2.1m)), Times.Once);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithCarPark()
    {
        _mockRepo.Setup(r => r.GetByIdAsync("ACB")).ReturnsAsync(MakeCarPark("ACB"));

        var result = await _controller.GetById("ACB");

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<Api.Dtos.CarParkDto>(ok.Value);
        Assert.Equal("ACB", dto.CarParkNo);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync("UNKNOWN")).ReturnsAsync((CarPark?)null);

        var result = await _controller.GetById("UNKNOWN");

        Assert.IsType<NotFoundResult>(result);
    }

    private static CarPark MakeCarPark(string no) => new()
    {
        CarParkNo = no,
        Address = $"Address for {no}",
        FreeParking = "SUN & PH FR 7AM-10.30PM",
        NightParking = true,
        GantryHeight = 2.4m,
        CarParkType = new CarParkType { Id = 1, Name = "SURFACE CAR PARK" },
        ParkingSystem = new ParkingSystem { Id = 1, Name = "ELECTRONIC PARKING" }
    };
}
