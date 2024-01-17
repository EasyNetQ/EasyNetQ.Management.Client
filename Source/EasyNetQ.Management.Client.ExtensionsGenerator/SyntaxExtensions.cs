using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyNetQ.Management.Client.ExtensionsGenerator;

public static class SyntaxExtensions
{
    public static ParameterSyntax GetParameterSyntax(this string parameter)
    {
        return SyntaxFactory.ParseParameterList($"({parameter})").Parameters.Single();
    }

    public static TypeSyntax GetTypeSyntax(this ITypeSymbol typeSymbol)
    {
        return SyntaxFactory.ParseTypeName(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
    }

    public static readonly SymbolDisplayFormat MethodDisplayFormat = SymbolDisplayFormat.MinimallyQualifiedFormat
        .AddParameterOptions(SymbolDisplayParameterOptions.IncludeExtensionThis)
        .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral);
    public static SeparatedSyntaxList<ParameterSyntax> GetParameterListSyntax(this IMethodSymbol method)
    {
        return (SyntaxFactory.ParseMemberDeclaration(method.ToDisplayString(MethodDisplayFormat)) as MethodDeclarationSyntax)!.ParameterList.Parameters;
    }
}
