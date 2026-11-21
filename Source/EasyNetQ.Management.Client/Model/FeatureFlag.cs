namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     A feature flag. <see cref="State"/> is enabled, disabled or state_changing; <see cref="Stability"/> required, stable or experimental.
/// </summary>
public record FeatureFlag(
    string Name,
    string State,
    string? Desc = null,
    string? DocUrl = null,
    string? Stability = null,
    string? ProvidedBy = null
);
