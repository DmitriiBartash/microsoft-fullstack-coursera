namespace JwtCore.Models;

public record TokenParts(string Header, string Payload, string Signature);
