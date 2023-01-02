namespace EasyNetQ.Management.Client.Model;

public record MessageRateDetails(
    double Rate,
    double AvgRate,
    double Avg,
    IReadOnlyList<MessageRateSample>? Samples
);
