namespace CarparkInfo.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;

    public ICollection<UserFavourite> Favourites { get; set; } = new List<UserFavourite>();
}
