using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Api.Contracts.Auth;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MinLength(2)] string DisplayName);
