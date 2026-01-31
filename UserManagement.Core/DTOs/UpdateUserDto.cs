using System.ComponentModel.DataAnnotations;

namespace UserManagement.Core.DTOs;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Phone] public string? PhoneNumber { get; set; }
}