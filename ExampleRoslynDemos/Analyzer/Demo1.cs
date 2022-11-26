using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExampleRoslynDemos.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Demo1 : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(ChangeGreeting, SyntaxKind.StringLiteralExpression);
        }

        private void ChangeGreeting(SyntaxNodeAnalysisContext obj)
        {
            var literal = obj.Node as LiteralExpressionSyntax;
            if (literal.Token.Text.Contains("World"))
            {
                obj.ReportDiagnostic(Diagnostic.Create(_descriptor, obj.Node.GetLocation(), literal.Token.Text));
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_descriptor);

        public const string IssueId = "Demo01";
        private readonly DiagnosticDescriptor _descriptor =
            new(IssueId, "The world is too big", "{0} is too big a greeting", "Test", DiagnosticSeverity.Warning, true);
    }
}
