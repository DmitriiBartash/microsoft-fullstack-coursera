using JwtCore.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var creator = new JwtCreator();
var decoder = new JwtDecoder();

Console.WriteLine("=== JWT Demo — create & decode (lab Step 4) ===\n");

string token = creator.Create(
    subject: "12345",
    name: "Alice Doe",
    role: "admin",
    lifetime: TimeSpan.FromHours(24));

Console.WriteLine("Generated JWT:");
Console.WriteLine(token);
Console.WriteLine();

try
{
    var principal = decoder.Validate(token);
    Console.WriteLine("Signature VALID. Decoded claims:");
    foreach (var claim in principal.Claims)
        Console.WriteLine($"  {claim.Type,-6} = {claim.Value}");
}
catch (Exception ex)
{
    Console.WriteLine("Validation FAILED: " + ex.Message);
}
