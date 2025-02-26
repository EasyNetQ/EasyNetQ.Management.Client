using EasyNetQ.Management.Client.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record UserInfo(
    string? Password,
    string? PasswordHash,
    IReadOnlyList<string> Tags,
    [property: JsonConverter(typeof(HashAlgorithmNameConverter))]
    HashAlgorithmName? HashingAlgorithm = null
)
{
    public static UserInfo ByPassword(string password)
        => new(password, null, Array.Empty<string>());
    public static UserInfo ByPasswordHash(string passwordHash, HashAlgorithmName? hashAlgorithmName = null)
        => new(null, passwordHash, Array.Empty<string>(), hashAlgorithmName);
    public static UserInfo ByPasswordAndHashAlgorithm(string password, HashAlgorithmName? hashAlgorithmName)
        => new(null, ComputePasswordHash(password, hashAlgorithmName), Array.Empty<string>(), hashAlgorithmName);

    public UserInfo AddTag(string tag)
        => Tags.Contains(tag) ? this : this with { Tags = Tags.Concat([tag]).ToArray() };

    public UserInfo WithTags(params string[] tags)
        => this with { Tags = tags };

    private static HashAlgorithm CreateHashAlgorithm(HashAlgorithmName? hashAlgorithmName = null)
    {
        return hashAlgorithmName switch
        {
            var name when name == null || name == HashAlgorithmName.SHA256 => SHA256.Create(),
            var name when name == HashAlgorithmName.SHA512 => SHA512.Create(),
            var name when name == HashAlgorithmName.MD5 => MD5.Create(),
            _ => throw new InvalidOperationException($"Unsupported hash algorithm: {hashAlgorithmName?.Name}")
        };
    }

    // https://www.rabbitmq.com/docs/passwords#this-is-the-algorithm
    public static string ComputePasswordHash(string password, HashAlgorithmName? hashAlgorithmName = null)
    {
        if (password == null)
        {
            throw new ArgumentNullException("password");
        }

        using var rand = RandomNumberGenerator.Create();
        using var hashAlgorithm = CreateHashAlgorithm(hashAlgorithmName);

        var salt = new byte[4];
        rand.GetBytes(salt);

        var utf8PasswordBytes = Encoding.UTF8.GetBytes(password);

        var concatenated = new byte[salt.Length + utf8PasswordBytes.Length];
        Buffer.BlockCopy(salt, 0, concatenated, 0, salt.Length);
        Buffer.BlockCopy(utf8PasswordBytes, 0, concatenated, salt.Length, utf8PasswordBytes.Length);

        var saltedHash = hashAlgorithm.ComputeHash(concatenated);

        concatenated = new byte[salt.Length + saltedHash.Length];
        Buffer.BlockCopy(salt, 0, concatenated, 0, salt.Length);
        Buffer.BlockCopy(saltedHash, 0, concatenated, salt.Length, saltedHash.Length);

        return Convert.ToBase64String(concatenated);
    }
}
