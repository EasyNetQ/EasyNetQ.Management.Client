namespace EasyNetQ.Management.Client.Model;

public class UserInfo
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string PasswordHash { get; set; }
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Creates <see cref="UserInfo"/> instance.
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="password">Password or password hash value.</param>
    /// <param name="isHashed">Flag shows if <paramref name="password">password</paramref> value is raw password or password hash.</param>
    /// <remarks>Hash should be calculated using RabbitMq hash computing algorithm.
    /// See https://www.rabbitmq.com/passwords.html.</remarks>
    /// <exception cref="ArgumentException"></exception>
    public UserInfo(string name, string password, bool isHashed = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("name is null or empty");
        }

        //Setting password_hash to "" will ensure the user cannot use a password to log in.
        //(From HTTP API documentation: https://cdn.rawgit.com/rabbitmq/rabbitmq-management/rabbitmq_v3_6_12/priv/www/api/index.html)
        if (string.IsNullOrEmpty(password) && !isHashed)
        {
            throw new ArgumentException("password is null or empty.");
        }

        Name = name;
        if (isHashed)
        {
            PasswordHash = password;
        }
        else
        {
            Password = password;
        }
    }

    public UserInfo AddTag(string tag)
    {
        if (!Tags.Contains(tag))
            Tags.Add(tag);
        return this;
    }
}
