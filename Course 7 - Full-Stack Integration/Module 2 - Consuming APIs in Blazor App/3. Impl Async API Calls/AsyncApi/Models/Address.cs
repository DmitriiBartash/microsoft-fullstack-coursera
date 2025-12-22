namespace AsyncApi.Models;

/// <summary>
/// Represents user's address
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string Suite { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zipcode { get; set; } = string.Empty;
    public Geo? Geo { get; set; }

    /// <summary>
    /// Returns formatted full address
    /// </summary>
    public string FullAddress => $"{Street}, {Suite}, {City} {Zipcode}";
}