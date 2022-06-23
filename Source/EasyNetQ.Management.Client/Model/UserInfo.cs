using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyNetQ.Management.Client.Model;

public class UserInfo
{
    private readonly string name;
    public string Password { get; private set; }
    public string PasswordHash { get; private set; }
    public string Tags => tagList.Any()
        ? string.Join(",", tagList)
        : string.Empty;

    private readonly ISet<string> allowedTags = new HashSet<string>
    {
        "administrator", "monitoring", "management", "policymaker"
    };

    private readonly ISet<string> tagList = new HashSet<string>();

    /// <summary>
    /// Creates <see cref="UserInfo"/> instance.
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="password">Password or password hash value.</param>
    /// <param name="isHashed">Flag shows if <param name="password">password</param> value is raw password or password hash.</param>
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
        this.name = name;
        if (isHashed)
        {
            this.PasswordHash = password;
        }
        else
        {
            this.Password = password;
        }
    }

    /// <summary>
    /// administrator: User can do everything monitoring can do, manage users, vhosts and permissions, close other user's connections, and manage policies and parameters for all vhosts.
    /// monitoring: User can access the management plugin and see all connections and channels as well as node-related information.
    /// management: User can access the management plugin
    /// policymaker: User can access the management plugin and manage policies and parameters for the vhosts they have access to.
    /// </summary>
    /// <param name="tag">One of the following tags: administrator, monitoring, management, policymaker</param>
    public UserInfo AddTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            throw new ArgumentException("tag is null or empty");
        }
        if (tagList.Contains(tag))
        {
            throw new ArgumentException($"tag '{tag}' has already been added");
        }
        if (!allowedTags.Contains(tag))
        {
            throw new ArgumentException(
                $"tag '{tag}' not recognised. Allowed tags are: {string.Join(", ", allowedTags)}");
        }

        tagList.Add(tag);
        return this;
    }

    public string GetName()
    {
        return name;
    }
}
