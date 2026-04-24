using JobPortal.Application.Common.Interfaces;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace JobPortal.Infrastructure.Identity;

/// <summary>
/// Argon2id password hashing service.
/// Parameters: memoryCost=65536 (64 MB), timeCost=3, parallelism=4, hashLength=32.
/// Includes application-level pepper from environment variable for defense-in-depth.
/// </summary>
public sealed class PasswordService(IConfiguration configuration) : IPasswordService
{
    private const int MemoryCost = 65536; // 64 MB
    private const int TimeCost = 3;
    private const int Parallelism = 4;
    private const int HashLength = 32;

    private string Pepper => configuration["Security:PasswordPepper"]
        ?? throw new InvalidOperationException("Security:PasswordPepper is not configured.");

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var pepperedPassword = ApplyPepper(password);

        var config = new Argon2Config
        {
            Type = Argon2Type.HybridAddressing, // Argon2id
            Version = Argon2Version.Nineteen,
            MemoryCost = MemoryCost,
            TimeCost = TimeCost,
            Lanes = Parallelism,
            Threads = Parallelism,
            HashLength = HashLength,
            Salt = RandomNumberGenerator.GetBytes(16),
            Password = System.Text.Encoding.UTF8.GetBytes(pepperedPassword)
        };

        using var argon2 = new Argon2(config);
        using var hashBytes = argon2.Hash();
        return config.EncodeString(hashBytes.Buffer);
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        var pepperedPassword = ApplyPepper(password);
        return Argon2.Verify(hash, pepperedPassword);
    }

    private string ApplyPepper(string password) => $"{password}{Pepper}";
}
