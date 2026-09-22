using EngineerOS.Application.Abstractions.Extraction;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EngineerOS.Infrastructure.Extraction;

public sealed class CSharpTypeExtractor : ICSharpTypeExtractor
{
    private readonly string _storageRoot;

    public CSharpTypeExtractor(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public async Task<IReadOnlyList<CSharpType>> ExtractAsync(
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

        var result = new List<CSharpType>();

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

            var namespaceName = root
                .DescendantNodes()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name
                .ToString()
                ?? string.Empty;

            foreach (var declaration in root
                         .DescendantNodes()
                         .OfType<BaseTypeDeclarationSyntax>())
            {
                result.Add(new CSharpType(
                    file,
                    namespaceName,
                    declaration.Identifier.Text,
                    GetKind(declaration),
                    GetVisibility(declaration),
                    GetModifier(declaration)));
            }
        }

        return result;
    }

    private static string GetKind(
        BaseTypeDeclarationSyntax declaration)
    {
        return declaration switch
        {
            ClassDeclarationSyntax => "Class",
            InterfaceDeclarationSyntax => "Interface",
            EnumDeclarationSyntax => "Enum",
            StructDeclarationSyntax => "Struct",
            RecordDeclarationSyntax => "Record",
            _ => "Other"
        };
    }

    private static string GetVisibility(
        BaseTypeDeclarationSyntax declaration)
    {
        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.PublicKeyword))
        {
            return "public";
        }

        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.InternalKeyword))
        {
            return "internal";
        }

        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.PrivateKeyword))
        {
            return "private";
        }

        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.ProtectedKeyword))
        {
            return "protected";
        }

        return "default";
    }

    private static string? GetModifier(
        BaseTypeDeclarationSyntax declaration)
    {
        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.SealedKeyword))
        {
            return "sealed";
        }

        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.AbstractKeyword))
        {
            return "abstract";
        }

        if (declaration.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.StaticKeyword))
        {
            return "static";
        }

        return null;
    }
}