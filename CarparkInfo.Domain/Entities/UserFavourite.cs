namespace CarparkInfo.Domain.Entities;

public class UserFavourite
{
    public Guid UserId { get; set; }
    public string CarParkNo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public CarPark CarPark { get; set; } = null!;
}
