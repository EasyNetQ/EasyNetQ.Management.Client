namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class HaParams
{
    public HaMode AssociatedHaMode { get; set; }
    public long ExactlyCount { get; set; }
    public string[] Nodes { get; set; }
}
