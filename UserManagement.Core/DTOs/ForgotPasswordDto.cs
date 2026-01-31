using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs;

public class ForgotPasswordDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}