using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyNetQ.Management.Client.ExtensionsGenerator;

/// <summary>
///     Implements every method of an interface on a partial class by calling the same method on one of its fields.
/// </summary>
public static class ForwardingGenerator
{
    public static ClassDeclarationSyntax GenerateForwardingClass(string className, INamedTypeSymbol interfaceType, string fieldName)
    {
        var methods = interfaceType.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method.MethodKind == MethodKind.Ordinary && !method.IsStatic)
            .Select(method => GenerateForwardingMethod(method, fieldName));

        return SyntaxFactory.ClassDeclaration(className)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .AddMembers(methods.ToArray<MemberDeclarationSyntax>());
    }

    private static MethodDeclarationSyntax GenerateForwardingMethod(IMethodSymbol method, string fieldName)
    {
        var parameters = method.GetParameterListSyntax();
        var invocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(fieldName),
                method.GetAccessName()),
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                parameters.Select(parameter => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Identifier))))));

        return SyntaxFactory.MethodDeclaration(method.ReturnType.GetTypeSyntax(), method.Name)
            .WithTypeParameterList(method.GetTypeParameterListSyntax())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(parameters))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(invocation))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("/// <inheritdoc />\n"));
    }
}
