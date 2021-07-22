using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVaribleAnalyzerCodeFix)), Shared]
    public class LocalVaribleAnalyzerCodeFix : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LocalVaribleAnalyzer.DiagnosticId); }
        }
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) // Obtain information about the diagnostics to fix
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false); // Get the syntax Tree

            var diagnostic = context.Diagnostics.First(); // Obtain the error to fix.
            var diagnosticSpan = diagnostic.Location.SourceSpan; // Get the specific location within tree associated with that error.

            VariableDeclaratorSyntax declaration = (VariableDeclaratorSyntax)root.FindNode(diagnosticSpan);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => RenameLocalVarible(context.Document, declaration, c), 
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        // Return the solution with changes of the rename.
        private static async Task<Solution> RenameLocalVarible(Document document, VariableDeclaratorSyntax localDeclaration, CancellationToken cancellationToken)
        {
            // Remove the leading trivia from the local declaration.
            SyntaxToken identifierToken = localDeclaration.Identifier; // Obtain the token
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken); // Obtain the semantic model associated with the document.
            ISymbol _VarSymbol = semanticModel.GetDeclaredSymbol(localDeclaration, cancellationToken); // Get the Symbol associated with the method.
            Solution _OriginalSolution = document.Project.Solution; // Original solution.

            string varibleName = identifierToken.Text;
            if (!varibleName.StartsWith("_"))
            {
                varibleName = "_" + varibleName;

                
            }
            return await Renamer.RenameSymbolAsync(_OriginalSolution, _VarSymbol, varibleName, _OriginalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);

        }
    }
}
