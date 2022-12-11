namespace EasyNetQ.Management.Client.Model;

public class LengthsDetails
{
    public double Rate { get; set; }
    public double AvgRate { get; set; }
    public double Avg { get; set; }

    public List<LengthsSample> Samples { get; set; }
}
