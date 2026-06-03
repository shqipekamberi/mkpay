using System.ComponentModel.DataAnnotations;

namespace MKPay.Core.DTOs.Account;

public class UpdateProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Url]
    public string? ProfilePictureUrl { get; set; }
}
