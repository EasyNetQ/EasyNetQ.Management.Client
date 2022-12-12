namespace EasyNetQ.Management.Client.Internals;

internal readonly struct RelativePath
{
    private readonly string[] segments;

    public RelativePath(string segment) => segments = new[] { segment };

    public string BuildEscaped() => string.Join("/", (segments ?? Array.Empty<string>()).Select(Uri.EscapeDataString));

    public static RelativePath operator /(RelativePath parent, string segment) => parent.Add(segment);
    public static RelativePath operator /(RelativePath parent, char segment) => parent.Add(segment + "");

    private RelativePath(string[] segments) => this.segments = segments;

    private RelativePath Add(string segment)
    {
        var old = segments ?? Array.Empty<string>();
        var current = new string[old.Length + 1];
        Array.Copy(old, 0, current, 0, old.Length);
        current[old.Length] = segment.Trim('/');
        return new RelativePath(current);
    }
}
