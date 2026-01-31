using System.ComponentModel.DataAnnotations;

namespace UserManagement.Core.DTOs;

public class ForgotPasswordDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}