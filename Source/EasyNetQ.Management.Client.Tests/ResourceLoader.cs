using System.Reflection;
using Newtonsoft.Json;

namespace EasyNetQ.Management.Client.Tests;

public class ResourceLoader
{
    public static T LoadObjectFromJson<T>(string fileToLoad, JsonSerializerSettings settings)
    {
        const string namespaceFormat = "EasyNetQ.Management.Client.Tests.Json.{0}";
        var resourceName = string.Format(namespaceFormat, fileToLoad);
        var assembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
        string contents;
        using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream == null)
            {
                throw new EasyNetQTestException("Couldn't load resource stream: " + resourceName);
            }
            using (var reader = new StreamReader(resourceStream))
            {
                contents = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<T>(contents, settings);
    }
}
