using System.ComponentModel.DataAnnotations;

namespace CarparkInfo.Api.Dtos;

public class AddFavouriteRequest
{
    [Required]
    public string CarParkNo { get; set; } = string.Empty;
}
