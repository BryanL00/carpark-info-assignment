namespace CarparkInfo.Domain.Models;

public class CarParkFilter
{
    public bool? FreeParking { get; set; }
    public bool? NightParking { get; set; }

    // Filter carparks whose gantry height is >= this value
    public decimal? VehicleHeight { get; set; }
}
