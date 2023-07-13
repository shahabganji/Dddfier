using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Dddfier.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DddfierAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SHGDDD001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var propertyExpressionSyntax = context.Node;

            if (propertyExpressionSyntax.Parent is not ClassDeclarationSyntax containingClassDeclaration)
                return;

            var className = containingClassDeclaration.Identifier.ToString();

            var symbol = context.SemanticModel.GetDeclaredSymbol(propertyExpressionSyntax);
            if (symbol is not IPropertySymbol propertySymbol)
                return;

            var idPropertyNames = new[] { "Id", "ID", $"{className}Id", $"{className}ID" };

            if (!idPropertyNames.Contains(propertySymbol.Name))
                return;

            if (!propertySymbol.Type.IsValueType)
                return;
            
            var propertyTypeName = propertySymbol.Type.Name;

            var diagnosticForProperty = Diagnostic.Create(Rule, propertyExpressionSyntax.GetLocation());

            context.ReportDiagnostic(diagnosticForProperty);
        }
    }
}
