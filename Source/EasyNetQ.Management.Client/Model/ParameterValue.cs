namespace EasyNetQ.Management.Client.Model;

/// <summary>
///     The body of a parameter. Under Native AOT, <see cref="Value"/> must be a type the client serializes:
///     <see cref="ParameterShovelValue"/>, <see cref="ParameterFederationValue"/>, a string, number or bool.
/// </summary>
public record ParameterValue(object Value);
