namespace CarparkInfo.Domain.Entities;

public class CarPark
{
    public string CarParkNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal XCoord { get; set; }
    public decimal YCoord { get; set; }
    public int CarParkTypeId { get; set; }
    public int ParkingSystemId { get; set; }
    public string ShortTermParking { get; set; } = string.Empty;

    // Raw value from CSV: "NO", "SUN & PH FR 7AM-10.30PM", etc.
    // Any value other than "NO" means free parking is available.
    public string FreeParking { get; set; } = string.Empty;

    public bool NightParking { get; set; }
    public int CarParkDecks { get; set; }
    public decimal GantryHeight { get; set; }
    public bool IsBasement { get; set; }

    public CarParkType CarParkType { get; set; } = null!;
    public ParkingSystem ParkingSystem { get; set; } = null!;
    public ICollection<UserFavourite> FavouritedBy { get; set; } = new List<UserFavourite>();
}
