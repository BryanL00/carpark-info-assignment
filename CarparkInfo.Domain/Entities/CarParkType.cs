namespace CarparkInfo.Domain.Entities;

public class CarParkType : ILookupEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CarPark> CarParks { get; set; } = new List<CarPark>();
}
