using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Api.Dtos;

public class UserFavouriteDto
{
    public string CarParkNo { get; set; } = string.Empty;
    public CarParkDto CarPark { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public static UserFavouriteDto From(UserFavourite f) => new()
    {
        CarParkNo = f.CarParkNo,
        CarPark = CarParkDto.From(f.CarPark),
        CreatedAt = f.CreatedAt
    };
}
