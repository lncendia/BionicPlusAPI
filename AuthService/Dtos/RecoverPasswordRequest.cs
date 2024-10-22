using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RecoverPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
