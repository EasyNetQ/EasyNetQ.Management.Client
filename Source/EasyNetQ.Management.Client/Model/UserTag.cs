namespace EasyNetQ.Management.Client.Model;

/// <summary>
/// Administrator: User can do everything monitoring can do, manage users, vhosts and permissions, close other user's connections, and manage policies and parameters for all vhosts.
/// Monitoring: User can access the management plugin and see all connections and channels as well as node-related information.
/// Management: User can access the management plugin
/// Policymaker: User can access the management plugin and manage policies and parameters for the vhosts they have access to.
/// Impersonator: User can forge a user-id.
/// </summary>
public enum UserTag
{
    Administrator,
    Monitoring,
    Management,
    Policymaker,
    Impersonator
}
