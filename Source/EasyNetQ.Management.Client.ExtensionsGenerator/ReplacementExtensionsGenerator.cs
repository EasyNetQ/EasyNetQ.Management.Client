using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyNetQ.Management.Client.ExtensionsGenerator;

public static class ReplacementExtensionsGenerator
{
    public static (ParameterSyntax, ParameterSyntax?, ExpressionSyntax) CreateReplacement(
        (string, string?, string) replacementStrings)
    {
        (string oldParameter, string? newParameter, string expression) = replacementStrings;

        var oldParameterSyntax = oldParameter.GetParameterSyntax();
        var newParameterSyntax = newParameter != null ? newParameter.GetParameterSyntax() : null;
        var expressionSyntax = SyntaxFactory.ParseExpression(expression);
        return (oldParameterSyntax, newParameterSyntax, expressionSyntax);
    }

    public static FileScopedNamespaceDeclarationSyntax AddReplacementExtensionsClass(
        this FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration,
        QualifiedNameSyntax extensionsClassName, INamedTypeSymbol t, ParameterSyntax thisParameter, params (string, string?, string)[] replacements)
    {
        var extensionsClass = GenerateReplacementExtensionsClass(extensionsClassName, t, thisParameter, replacements.Select(r => CreateReplacement(r)));
        if (extensionsClass != null)
            fileScopedNamespaceDeclaration = fileScopedNamespaceDeclaration.AddMembers(extensionsClass);
        return fileScopedNamespaceDeclaration;
    }

    public static ClassDeclarationSyntax? GenerateReplacementExtensionsClass(
        QualifiedNameSyntax extensionsClassName, INamedTypeSymbol t, ParameterSyntax thisParameter, IEnumerable<(ParameterSyntax, ParameterSyntax?, ExpressionSyntax)> replacements)
    {
        var extensionMethods = t.GetMembers().OfType<IMethodSymbol>()
            .Where(method => replacements.All(replacement => method.Parameters.Any(parameter => parameter.Name == replacement.Item1.Identifier.Text)))
            .Select(method => GenerateReplacementExtensionMethod(method, thisParameter, replacements)).ToArray();
        if (extensionMethods.Length == 0)
            return null;

        return SyntaxFactory.ClassDeclaration(extensionsClassName.Right.Identifier.Text)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .AddMembers(extensionMethods)
            .WithLeadingTrivia(
                SyntaxFactory.Comment($"// Replacement extensions of {t.Name}'s methods"),
                SyntaxFactory.Comment("// (" +
                    string.Join(", ", replacements.Where(r => r.Item2 != null).Select(r => $"{r.Item2!.Type} {r.Item2.Identifier}")) + ") -> (" +
                    string.Join(", ", replacements.Select(r => $"{r.Item1.Type} {r.Item1.Identifier}")) + ")"));
    }

    private static MethodDeclarationSyntax GenerateReplacementExtensionMethod(
        IMethodSymbol method, ParameterSyntax thisParameter, IEnumerable<(ParameterSyntax, ParameterSyntax?, ExpressionSyntax)> replacements)
    {
        var methodName = method.Name;

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
        foreach (var replacement in replacements)
        {
            (var oldParameterSyntax, var newParameterSyntax, var expressionSyntax) = replacement;
            if (newParameterSyntax == null)
                parameters = SyntaxFactory.SeparatedList(parameters.Where(p => !p.IsEquivalentTo(oldParameterSyntax)));
            else
                parameters = SyntaxFactory.SeparatedList(parameters.Select(p => p.IsEquivalentTo(oldParameterSyntax) ? newParameterSyntax : p));
            argumentExpressions = argumentExpressions.Select(e => e.ToString() == oldParameterSyntax.Identifier.Text ? expressionSyntax : e);
        }

        ExpressionSyntax expression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(thisParameter.Identifier.Text),
                method.GetAccessName()
            ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(argumentExpressions.Select(a => SyntaxFactory.Argument(a))))
        );

        return SyntaxFactory.MethodDeclaration(method.ReturnType.GetTypeSyntax(), methodName)
            .WithTypeParameterList(method.GetTypeParameterListSyntax())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(parameters))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }
}
