using System.Security.Cryptography;
using System.Text.Json;

var rsa = RSA.Create(2048);

var privPem = rsa.ExportRSAPrivateKeyPem();
var pubPem  = rsa.ExportSubjectPublicKeyInfoPem();

var pepperBytes = RandomNumberGenerator.GetBytes(32);
var pepper = Convert.ToHexString(pepperBytes).ToLowerInvariant();

var secrets = new
{
    Jwt = new { PublicKey = pubPem, PrivateKey = privPem },
    Security = new { PasswordPepper = pepper }
};

var json = JsonSerializer.Serialize(secrets, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText("appsettings.Secrets.json", json);

Console.WriteLine("OK");
Console.WriteLine($"Pepper prefix: {pepper[..8]}");
Console.WriteLine($"Pub key lines: {pubPem.Split('\n').Length}");
