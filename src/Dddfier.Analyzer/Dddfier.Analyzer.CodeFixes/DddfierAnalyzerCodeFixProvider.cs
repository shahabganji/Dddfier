using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dddfier.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DddfierAnalyzerCodeFixProvider)), Shared]
    public class DddfierAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DddfierAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root is null)
                return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var classDeclarationSyntax =
                root.FindToken(diagnosticSpan.Start)
                    .Parent?
                    .AncestorsAndSelf()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault();

            if (classDeclarationSyntax is null)
                return;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: _ =>
                        AddWithIdAttribute(context.Document, classDeclarationSyntax),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private static async Task<Document> AddWithIdAttribute(
            Document contextDocument,
            ClassDeclarationSyntax declaration)
        {
            var attribute = SyntaxFactory.AttributeList
            (
                SyntaxFactory.SingletonSeparatedList
                (
                    SyntaxFactory.Attribute
                    (
                        SyntaxFactory.GenericName
                            (
                                SyntaxFactory.Identifier("WithIdOf")
                            )
                            .WithTypeArgumentList
                            (
                                SyntaxFactory.TypeArgumentList
                                    (
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>
                                        (
                                            SyntaxFactory.IdentifierName("int")
                                        )
                                    )
                                    .WithLessThanToken
                                    (
                                        SyntaxFactory.Token(SyntaxKind.LessThanToken)
                                    )
                                    .WithGreaterThanToken
                                    (
                                        SyntaxFactory.Token(SyntaxKind.GreaterThanToken)
                                    )
                            )
                    )));

            var newClassDeclaration = declaration.WithAttributeLists(
                new SyntaxList<AttributeListSyntax>(attribute));

            return contextDocument.WithSyntaxRoot(newClassDeclaration);
        }
    }
}
