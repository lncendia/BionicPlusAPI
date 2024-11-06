using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class ChangePasswordRequest
{

    [Required, DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}