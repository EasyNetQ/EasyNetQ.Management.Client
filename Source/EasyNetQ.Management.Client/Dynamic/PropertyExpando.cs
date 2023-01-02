using System.Dynamic;

namespace EasyNetQ.Management.Client.Dynamic;

public abstract class PropertyExpando : DynamicObject
{
    protected PropertyExpando(IDictionary<string, object?> properties)
    {
        Properties = properties ?? throw new ArgumentNullException(nameof(properties), "The argument properties must not be null");
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        => Properties.TryGetValue(binder.Name, out result);

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        Properties[binder.Name] = value;
        return true;
    }

    protected T? GetPropertyOrDefault<T>(string propertyName)
        => Properties.TryGetValue(propertyName, out var propertyValue) && propertyValue != null ? (T)propertyValue : default;

    protected IDictionary<string, object?> Properties { get; }
}
