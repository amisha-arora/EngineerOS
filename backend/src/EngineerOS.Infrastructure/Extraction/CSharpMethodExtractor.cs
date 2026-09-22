using EngineerOS.Application.Abstractions.Extraction;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EngineerOS.Infrastructure.Extraction;

public sealed class CSharpMethodExtractor : ICSharpMethodExtractor
{
    private readonly string _storageRoot;

    public CSharpMethodExtractor(string storageRoot)
    {
        _storageRoot = storageRoot;
    }

    public async Task<IReadOnlyList<CSharpMethod>> ExtractAsync(
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

        var methods = new List<CSharpMethod>();

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

            foreach (var method in root
                         .DescendantNodes()
                         .OfType<MethodDeclarationSyntax>())
            {
                var containingType = method
                    .Ancestors()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .FirstOrDefault()?
                    .Identifier
                    .Text
                    ?? string.Empty;

                var parameters = method.ParameterList.Parameters
                    .Select(parameter => new CSharpMethodParameter(
                        parameter.Identifier.Text,
                        parameter.Type?.ToString() ?? "unknown"))
                    .ToList();

                var lineNumber = syntaxTree
                    .GetLineSpan(method.Span)
                    .StartLinePosition
                    .Line + 1;

                methods.Add(new CSharpMethod(
                    file,
                    containingType,
                    method.Identifier.Text,
                    method.ReturnType.ToString(),
                    GetVisibility(method),
                    parameters,
                    lineNumber));
            }
        }

        return methods;
    }

    private static string GetVisibility(
        MethodDeclarationSyntax method)
    {
        if (method.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.PublicKeyword))
        {
            return "public";
        }

        if (method.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.InternalKeyword))
        {
            return "internal";
        }

        if (method.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.PrivateKeyword))
        {
            return "private";
        }

        if (method.Modifiers.Any(
                modifier => modifier.RawKind == (int)SyntaxKind.ProtectedKeyword))
        {
            return "protected";
        }

        return "default";
    }
}