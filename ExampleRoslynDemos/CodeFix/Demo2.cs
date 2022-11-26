using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ExampleRoslynDemos.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Demo2))]
    public class Demo2:CodeFixProvider
    {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault();

            if (diagnostic == null)
            {
                return Task.CompletedTask;
            }

            context.RegisterCodeFix(CodeAction.Create("Make greeting appropriate",c => AppropriateGreeting(context.Document, diagnostic),"Demo01CodeFixProvider"),diagnostic);
            return Task.CompletedTask;
        }

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private async Task<Document> AppropriateGreeting(Document original, Diagnostic diagnostic)
        {
            var info = string.Empty;
            try
            {
                var root = await original.GetSyntaxRootAsync();
                info += "Get Root; ";
                var stringToken = root.FindNode(diagnostic.Location.SourceSpan) as LiteralExpressionSyntax;

                if (stringToken == null)
                {
                    throw new Exception("Token isn't a literal expression.");
                }

                info += "StringToken Isn't Null" ;
                var newText = stringToken.Token.Text.Trim('"').Replace("World", "Dot Net Notts");
                var newToken = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(newText));

                return original.WithSyntaxRoot(root.ReplaceNode(stringToken, newToken));
            }
            catch (Exception ex)
            {
                throw new Exception(info);
            }

        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("Demo01");
    }
}
