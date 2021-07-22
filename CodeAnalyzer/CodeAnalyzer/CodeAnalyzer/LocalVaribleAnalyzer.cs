using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalVaribleAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AddLocalPrefix";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement); // Syntax Kind = What are we looking for to trigger this analyser?
                                                                                                 // When we see a local declaration call this analyser.
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _localDeclaration = (LocalDeclarationStatementSyntax)context.Node;


            foreach (VariableDeclaratorSyntax varibleDeclaratorNode in _localDeclaration.Declaration.Variables) // Can have multiple varibles declared in one line.
            {
                if (!varibleDeclaratorNode.Identifier.Text.StartsWith("_"))
                {
                    Diagnostic report = Diagnostic.Create(Rule, varibleDeclaratorNode.Identifier.GetLocation(), varibleDeclaratorNode.Identifier.Text);
                    context.ReportDiagnostic(report);
                }
            }
        }

        private void AddTwoNumbers()
        {
            int NumberOne;
            int NumberTwo;

            NumberOne = 1; NumberTwo = 2;

            Console.WriteLine(NumberOne + NumberTwo);
        }
    }
}
