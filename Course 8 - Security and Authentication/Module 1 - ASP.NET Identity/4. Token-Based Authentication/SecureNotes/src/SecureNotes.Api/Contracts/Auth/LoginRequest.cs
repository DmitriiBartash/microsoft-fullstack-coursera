using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Api.Contracts.Auth;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
