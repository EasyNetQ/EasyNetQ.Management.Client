using System.Dynamic;

namespace EasyNetQ.Management.Client.Dynamic;

public abstract class PropertyExpando : DynamicObject
{
    private readonly IDictionary<string, object> properties;

    protected PropertyExpando(IDictionary<string, object> properties)
    {
        this.properties = properties ?? throw new ArgumentNullException(nameof(properties), "The argument properties must not be null");
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        if (!properties.Keys.Contains(binder.Name))
        {
            result = null;
            return false;
        }

        result = properties[binder.Name];
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        properties[binder.Name] = value;
        return true;
    }

    protected T GetPropertyOrDefault<T>(string propertyName)
    {
        if (properties.Keys.Contains(propertyName) && properties[propertyName] != null)
        {
            return (T)properties[propertyName];
        }
        return default;
    }

    protected IDictionary<string, object> Properties => properties;
}
