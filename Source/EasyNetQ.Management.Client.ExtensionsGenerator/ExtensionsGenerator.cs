using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyNetQ.Management.Client.ExtensionsGenerator;

[Generator]
public class ExtensionsGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;

        string typeName = "EasyNetQ.Management.Client.IManagementClient";
        string typeExtensionsName = $"{typeName}Extensions";

        var typeNames = new List<string> { typeName, typeExtensionsName };
        var types = typeNames.Select(tn => compilation.GetTypeByMetadataName(tn) ?? throw new KeyNotFoundException(tn));

        Dictionary<string, CompilationUnitSyntax> compilationUnits = new();

        var thisParameter = "this IManagementClient client".GetParameterSyntax();
        QualifiedNameSyntax extensionsClassName = (SyntaxFactory.ParseTypeName(typeExtensionsName) as QualifiedNameSyntax)!;

        {
            var fileScopedNamespaceDeclaration = SyntaxFactory.FileScopedNamespaceDeclaration(extensionsClassName.Left)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)));

            foreach (var t in types)
            {
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", "Vhost vhost", "vhost.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", null, "exchange.Vhost"),
                        ("string exchangeName", "ExchangeName exchange", "exchange.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", null, "queue.Vhost"),
                        ("string queueName", "QueueName queue", "queue.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", null, "exchange.Vhost"),
                        ("string exchangeName", "ExchangeName exchange", "exchange.Name"),
                        ("string queueName", "QueueName queue", "queue.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", null, "sourceExchange.Vhost"),
                        ("string sourceExchangeName", "ExchangeName sourceExchange", "sourceExchange.Name"),
                        ("string destinationExchangeName", "ExchangeName destinationExchange", "destinationExchange.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string connectionName", "Connection connection", "connection.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string userName", "User user", "user.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string vhostName", "Vhost vhost", "vhost.Name"),
                        ("string userName", "User user", "user.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string componentName", null, "parameter.Component"),
                        ("string vhostName", null, "parameter.Vhost"),
                        ("string parameterName", "Parameter parameter", "parameter.Name")
                     );
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddReplacementExtensionsClass(extensionsClassName, t, thisParameter,
                        ("string componentName", null, "parameter.Component"),
                        ("string vhostName", null, "parameter.Vhost"),
                        ("string parameterName", null, "parameter.Name"),
                        ("object parameterValue", "Parameter parameter", "parameter.Value")
                     );
            }

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("EasyNetQ.Management.Client.Model")))
                .AddMembers(fileScopedNamespaceDeclaration)
                .NormalizeWhitespace(eol: "\n");

            compilationUnits[$"{extensionsClassName.Right}Replacement"] = compilationUnit;

            compilation = compilation.AddSyntaxTrees(compilationUnit.SyntaxTree);
        }

        {
            var fileScopedNamespaceDeclaration = SyntaxFactory.FileScopedNamespaceDeclaration(extensionsClassName.Left)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)));

            foreach (var t in types)
            {
                fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration
                    .AddSyncExtensionsClass(extensionsClassName, t, thisParameter);
            }

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("EasyNetQ.Management.Client.Model")))
                .AddMembers(fileScopedNamespaceDeclaration)
                .NormalizeWhitespace(eol: "\n");

            compilationUnits[$"{extensionsClassName.Right}Sync"] = compilationUnit;
        }

        foreach (var kvpair in compilationUnits)
        {
            context.AddSource($"{kvpair.Key}.g.cs", kvpair.Value.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one

        // if (!System.Diagnostics.Debugger.IsAttached)
        // {
        //     System.Diagnostics.Debugger.Launch();
        // }
    }
}
