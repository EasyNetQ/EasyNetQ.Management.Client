using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Serialization;

internal class HashAlgorithmNameConverter : JsonConverter<HashAlgorithmName>
{
    static readonly HashAlgorithmName[] Supported = [ HashAlgorithmName.SHA256, HashAlgorithmName.SHA512, HashAlgorithmName.MD5 ];
    const string Prefix = "rabbit_password_hashing_";

    public override HashAlgorithmName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(HashAlgorithmName));

        string? stringValue = reader.GetString();
        if (stringValue != null && stringValue.StartsWith(Prefix))
        {
            var suffix = stringValue.Substring(Prefix.Length);
            if (suffix.ToLower() == suffix)
            {
                var hashAlgorithmName = new HashAlgorithmName(suffix.ToUpper());
                if (Supported.Contains(hashAlgorithmName))
                {
                    return hashAlgorithmName;
                }
            }
        }

        throw new JsonException($"Unsupported hash algorithm: {stringValue}");
    }

    public override void Write(Utf8JsonWriter writer, HashAlgorithmName value, JsonSerializerOptions options)
    {
        if (value.Name != null && Supported.Contains(value))
        {
            writer.WriteStringValue($"{Prefix}{value.Name.ToLower()}");
            return;
        }

        throw new JsonException($"Unsupported hash algorithm: {value.Name}");
    }
}
