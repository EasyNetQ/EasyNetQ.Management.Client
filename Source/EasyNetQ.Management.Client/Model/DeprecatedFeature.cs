namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     A deprecated feature. <see cref="DeprecationPhase"/> is permitted_by_default, denied_by_default, disconnected or removed.
/// </summary>
public record DeprecatedFeature(
    string Name,
    string? State = null,
    string? DeprecationPhase = null,
    string? Desc = null,
    string? DocUrl = null,
    string? ProvidedBy = null
);
