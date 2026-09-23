using EngineerOS.Application.Abstractions.Extraction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EngineerOS.Infrastructure.Extraction;

public sealed class CSharpDependencyExtractor : ICSharpDependencyExtractor
{
    private readonly string _storageRoot;

    public CSharpDependencyExtractor(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public async Task<IReadOnlyList<CSharpDependency>> ExtractAsync(
        Guid repositoryId,
        CancellationToken cancellationToken)
    {
        var extractedDirectory = Path.Combine(
            _storageRoot,
            "repositories",
            repositoryId.ToString(),
            "extracted");

        if (!Directory.Exists(extractedDirectory))
        {
            return [];
        }

        var dependencies = new List<CSharpDependency>();

        foreach (var fullPath in Directory.EnumerateFiles(
                     extractedDirectory,
                     "*.cs",
                     SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sourceCode = await File.ReadAllTextAsync(
                fullPath,
                cancellationToken);

            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceCode,
                cancellationToken: cancellationToken);

            var root = await syntaxTree.GetRootAsync(
                cancellationToken);

            var file = Path.GetRelativePath(
                extractedDirectory,
                fullPath).Replace('\\', '/');

            foreach (var type in root
                         .DescendantNodes()
                         .OfType<TypeDeclarationSyntax>())
            {
                ExtractBaseTypes(
                    type,
                    file,
                    syntaxTree,
                    dependencies);

                ExtractFieldDependencies(
                    type,
                    file,
                    syntaxTree,
                    dependencies);

                ExtractPropertyDependencies(
                    type,
                    file,
                    syntaxTree,
                    dependencies);

                ExtractConstructorDependencies(
                    type,
                    file,
                    syntaxTree,
                    dependencies);
            }
        }

        return dependencies;
    }

    private static void ExtractBaseTypes(
        TypeDeclarationSyntax type,
        string file,
        SyntaxTree syntaxTree,
        List<CSharpDependency> dependencies)
    {
        if (type.BaseList is null)
        {
            return;
        }

        foreach (var baseType in type.BaseList.Types)
        {
            var targetType = baseType.Type.ToString();

            var relationship = targetType.StartsWith("I",
                StringComparison.Ordinal) &&
                targetType.Length > 1 &&
                char.IsUpper(targetType[1])
                ? "Implements"
                : "Inherits";

            dependencies.Add(new CSharpDependency(
                file,
                type.Identifier.Text,
                targetType,
                relationship,
                GetLineNumber(syntaxTree, baseType.Span)));
        }
    }

    private static void ExtractFieldDependencies(
        TypeDeclarationSyntax type,
        string file,
        SyntaxTree syntaxTree,
        List<CSharpDependency> dependencies)
    {
        foreach (var field in type.Members
                     .OfType<FieldDeclarationSyntax>())
        {
            var targetType = field.Declaration.Type.ToString();

            dependencies.Add(new CSharpDependency(
                file,
                type.Identifier.Text,
                targetType,
                "FieldDependency",
                GetLineNumber(syntaxTree, field.Span)));
        }
    }

    private static void ExtractPropertyDependencies(
        TypeDeclarationSyntax type,
        string file,
        SyntaxTree syntaxTree,
        List<CSharpDependency> dependencies)
    {
        foreach (var property in type.Members
                     .OfType<PropertyDeclarationSyntax>())
        {
            var targetType = property.Type.ToString();

            dependencies.Add(new CSharpDependency(
                file,
                type.Identifier.Text,
                targetType,
                "PropertyDependency",
                GetLineNumber(syntaxTree, property.Span)));
        }
    }

    private static void ExtractConstructorDependencies(
        TypeDeclarationSyntax type,
        string file,
        SyntaxTree syntaxTree,
        List<CSharpDependency> dependencies)
    {
        foreach (var constructor in type.Members
                     .OfType<ConstructorDeclarationSyntax>())
        {
            foreach (var parameter in constructor
                         .ParameterList
                         .Parameters)
            {
                var targetType = parameter.Type?.ToString();

                if (string.IsNullOrWhiteSpace(targetType))
                {
                    continue;
                }

                dependencies.Add(new CSharpDependency(
                    file,
                    type.Identifier.Text,
                    targetType,
                    "ConstructorDependency",
                    GetLineNumber(syntaxTree, parameter.Span)));
            }
        }
    }

    private static int GetLineNumber(
        SyntaxTree syntaxTree,
        TextSpan span)
    {
        return syntaxTree
            .GetLineSpan(span)
            .StartLinePosition
            .Line + 1;
    }
}