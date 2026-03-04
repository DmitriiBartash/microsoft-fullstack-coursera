using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Api.Contracts.Auth;

public record RefreshRequest(
    [Required] string AccessToken,
    [Required] string RefreshToken);
