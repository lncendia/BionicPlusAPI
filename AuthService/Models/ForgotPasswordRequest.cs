﻿using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class ForgotPasswordRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;   
    
    [Required]
    public string Captcha { get; set; } = string.Empty;
}