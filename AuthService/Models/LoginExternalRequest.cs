﻿using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class LoginExternalRequest
{
    [Required]
    public string Provider { get; set; } = string.Empty;
        
    [Required]
    public string IdentityToken { get; set; } = string.Empty;
}