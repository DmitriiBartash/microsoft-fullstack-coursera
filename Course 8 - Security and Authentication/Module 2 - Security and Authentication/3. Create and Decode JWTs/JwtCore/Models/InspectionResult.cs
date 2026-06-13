namespace JwtCore.Models;

// Outcome of inspecting a token: contents are always exposed; validity is reported separately.
public class InspectionResult
{
    public bool IsValid { get; init; }
    public bool IsReadable { get; init; }
    public string Status { get; init; } = "malformed"; // valid | expired | bad-signature | invalid | malformed
    public string Message { get; init; } = string.Empty;
    public TokenParts? Parts { get; init; }
    public IReadOnlyList<ClaimItem> Claims { get; init; } = [];
}
