namespace EasyNetQ.Management.Client.Model;

#nullable disable

public class Parameter
{
    public string Vhost { get; set; }
    public string Component { get; set; }
    public string Name { get; set; }
    public object Value { get; set; }
}
