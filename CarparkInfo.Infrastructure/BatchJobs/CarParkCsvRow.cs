namespace CarparkInfo.Infrastructure.BatchJobs;

public class CarParkCsvRow
{
    public string CarParkNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal XCoord { get; set; }
    public decimal YCoord { get; set; }
    public string CarParkType { get; set; } = string.Empty;
    public string TypeOfParkingSystem { get; set; } = string.Empty;
    public string ShortTermParking { get; set; } = string.Empty;
    public string FreeParking { get; set; } = string.Empty;
    public string NightParking { get; set; } = string.Empty;
    public int CarParkDecks { get; set; }
    public decimal GantryHeight { get; set; }
    public string CarParkBasement { get; set; } = string.Empty;
}
