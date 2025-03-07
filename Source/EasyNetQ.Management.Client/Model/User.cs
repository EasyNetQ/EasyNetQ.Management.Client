using EasyNetQ.Management.Client.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record User(
    string Name,
    string PasswordHash,
    IReadOnlyList<string> Tags,
    [property: JsonConverter(typeof(HashAlgorithmNameConverter))]
    HashAlgorithmName? HashingAlgorithm = null,
    Limits? Limits = null
);
