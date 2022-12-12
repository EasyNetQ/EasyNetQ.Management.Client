namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class MessageRateDetails
{
    public double Rate { get; set; }
    public double AvgRate { get; set; }
    public double Avg { get; set; }

    public List<MessageRateSample> Samples { get; set; }
}
