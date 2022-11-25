using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DemoHints
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Demo1Hints : DiagnosticAnalyzer
    {
        public const string RoslynDemoCategory = "DotNetNotts";
        public static string RDAnalyzerInherit = RoslynDemoCategory + "0001";
        public static string RDAnalyzerAttribute = RoslynDemoCategory + "0002";
        public static string RDAnalyzerDescriptor = RoslynDemoCategory + "0003";
        public static string RDAnalyzerInitialize = RoslynDemoCategory + "0004";
        //CreateDescriptor
        //return SupportedDiagnostics
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSymbolAction(ClassLevelChecks, SymbolKind.NamedType);
        }

        //Inherit 
        //Add Attribute
        private static void ClassLevelChecks(SymbolAnalysisContext obj)
        {
            var node = (INamedTypeSymbol)obj.Symbol;
            if (node.Name != "Demo1")
            {
                return;
            }

            if (node.BaseType?.Name == nameof(Object))
            {
                obj.ReportDiagnostic(Diagnostic.Create(InheritIssue, obj.Symbol.Locations.FirstOrDefault(), obj.Symbol.Name));
                return;
            }

            if (!node.GetAttributes().Any(ad => ad.AttributeClass.Name == "DiagnosticAnalyzerAttribute"))
            {
                obj.ReportDiagnostic(Diagnostic.Create(AttributeUsageIssue, obj.Symbol.Locations.FirstOrDefault(), obj.Symbol.Name));
                return;
            }

            if (!node.MemberNames.Contains("_descriptor"))
            {
                obj.ReportDiagnostic(Diagnostic.Create(DescriptorIssue, obj.Symbol.Locations.FirstOrDefault(),obj.Symbol.Name));
                return;
            }

            var initializeSyntax = node.GetMembers("Initialize").SelectMany(m => m.DeclaringSyntaxReferences.Select(d => d.GetSyntax())).OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (initializeSyntax?.Body?.Statements.OfType<ThrowStatementSyntax>().Any() ?? true)
            {
                obj.ReportDiagnostic(Diagnostic.Create(InitializeIssue, initializeSyntax.GetLocation(),obj.Symbol.Name));
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => _descriptors;

        private static ImmutableArray<DiagnosticDescriptor> _descriptors => ImmutableArray.Create(InheritIssue, AttributeUsageIssue, DescriptorIssue, InitializeIssue);

        private static DiagnosticDescriptor InheritIssue = new(RDAnalyzerInherit,
            HintResources.RDAnalyzerInheritTitle,
            HintResources.RDAnalyzerInheritMessageFormat,
            RoslynDemoCategory, DiagnosticSeverity.Info,
            true);

        private static DiagnosticDescriptor AttributeUsageIssue = new(RDAnalyzerAttribute,
            HintResources.RDAnalyzerAttributeTitle,
            HintResources.RDAnalyzerAttributeMessageFormat,
            RoslynDemoCategory, DiagnosticSeverity.Info,
            true);

        private static DiagnosticDescriptor DescriptorIssue = new(RDAnalyzerDescriptor,
            HintResources.RDAnalyzerDescriptorTitle,
            HintResources.RDAnalyzerDescriptorMessageFormat,
            RoslynDemoCategory, DiagnosticSeverity.Info,
            true);

        private static DiagnosticDescriptor InitializeIssue = new(RDAnalyzerInitialize,
            HintResources.RDAnalyzerInitializeTitle,
            HintResources.RDAnalyzerInitializeMessageFormat,
            RoslynDemoCategory, DiagnosticSeverity.Info,
            true);
    }
}
