using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyNetQ.Management.Client.ExtensionsGenerator;

public static class SyncExtensionsGenerator
{
    private static bool IsAwaitable(this IMethodSymbol method, out ITypeSymbol? awaitableResultType)
    {
        ITypeSymbol? temp = null;
        bool result =
            method.ReturnType is INamedTypeSymbol returnType
            && returnType.GetMembers("GetAwaiter").OfType<IMethodSymbol>().Any(
                getAwaiterMethod => getAwaiterMethod.Parameters.IsEmpty
                    && getAwaiterMethod.ReturnType is INamedTypeSymbol getAwaiterReturnType
                    && getAwaiterReturnType.GetMembers("GetResult").OfType<IMethodSymbol>().Any(
                        getResultMethod => getResultMethod.Parameters.IsEmpty ? ((temp = getResultMethod.ReturnType) != null) : false));
        awaitableResultType = temp;
        return result;
    }

    public static FileScopedNamespaceDeclarationSyntax AddSyncExtensionsClass(
        this FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration,
        QualifiedNameSyntax extensionsClassName, INamedTypeSymbol t, ParameterSyntax thisParameter)
    {
        var extensionsClass = GenerateSyncExtensionsClass(extensionsClassName, t, thisParameter);
        if (extensionsClass != null)
            fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration.AddMembers(extensionsClass);
        return fileScopedNamespaceDeclaration;
    }

    private static ClassDeclarationSyntax? GenerateSyncExtensionsClass(
        QualifiedNameSyntax extensionsClassName, INamedTypeSymbol t, ParameterSyntax thisParameter)
    {
        var extensionMethods = t.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method.IsAwaitable(out _))
            .Select(method => GenerateSyncExtensionMethod(method, thisParameter)).ToArray();
        if (extensionMethods.Length == 0)
            return null;

        return SyntaxFactory.ClassDeclaration(extensionsClassName.Right.Identifier.Text)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .AddMembers(extensionMethods)
            .WithLeadingTrivia(
                SyntaxFactory.Comment($"// Sync extensions of {t.Name}'s awaitable methods"));
    }

    private static MethodDeclarationSyntax GenerateSyncExtensionMethod(
        IMethodSymbol method, ParameterSyntax thisParameter)
    {
        var methodName = method.Name.EndsWith("Async") ? method.Name[..^5] : (method.Name + "Sync");

        var parameters = method.GetParameterListSyntax();
        if (method.IsExtensionMethod)
        {
            thisParameter = parameters[0];
        }
        else
        {
            parameters = parameters.Insert(0, thisParameter);
        }

        IEnumerable<ExpressionSyntax> argumentExpressions = parameters.Skip(1).Select(parameter => SyntaxFactory.IdentifierName(parameter.Identifier));

        ExpressionSyntax expression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(thisParameter.Identifier.Text),
                method.GetAccessName()
            ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(argumentExpressions.Select(a => SyntaxFactory.Argument(a))))
        );
        expression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                expression,
                SyntaxFactory.IdentifierName("GetAwaiter")
            )
        );
        expression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                expression,
                SyntaxFactory.IdentifierName("GetResult")
            )
        );

        method.IsAwaitable(out var awaitableResultType);

        return SyntaxFactory.MethodDeclaration(awaitableResultType!.GetTypeSyntax(), methodName)
            .WithTypeParameterList(method.GetTypeParameterListSyntax())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(parameters))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }
}
