namespace EasyNetQ.Management.Client.Model;

public record HaParams(HaMode AssociatedHaMode, long? ExactlyCount = null, IReadOnlyList<string>? Nodes = null);
