using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Api.Dtos;

public class CarParkDto
{
    public string CarParkNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal XCoord { get; set; }
    public decimal YCoord { get; set; }
    public string CarParkType { get; set; } = string.Empty;
    public string ParkingSystem { get; set; } = string.Empty;
    public string ShortTermParking { get; set; } = string.Empty;
    public string FreeParking { get; set; } = string.Empty;
    public bool NightParking { get; set; }
    public int CarParkDecks { get; set; }
    public decimal GantryHeight { get; set; }
    public bool IsBasement { get; set; }

    public static CarParkDto From(CarPark c) => new()
    {
        CarParkNo = c.CarParkNo,
        Address = c.Address,
        XCoord = c.XCoord,
        YCoord = c.YCoord,
        CarParkType = c.CarParkType?.Name ?? string.Empty,
        ParkingSystem = c.ParkingSystem?.Name ?? string.Empty,
        ShortTermParking = c.ShortTermParking,
        FreeParking = c.FreeParking,
        NightParking = c.NightParking,
        CarParkDecks = c.CarParkDecks,
        GantryHeight = c.GantryHeight,
        IsBasement = c.IsBasement
    };
}
