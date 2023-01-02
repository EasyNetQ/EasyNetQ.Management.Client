namespace EasyNetQ.Management.Client.Model;

public record LengthsDetails(
    double Rate,
    double AvgRate,
    double Avg,
    IReadOnlyList<LengthsSample>? Samples = null
);
