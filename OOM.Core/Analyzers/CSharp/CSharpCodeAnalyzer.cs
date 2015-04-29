using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using OOM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OOM.Core.Analyzers.CSharp
{
    public class CSharpCodeAnalyzer : CodeAnalyzer
    {
        public CSharpCodeAnalyzer(CodeAnalyzerConfiguration configuration)
            : base(configuration)
        {
            
        }

        public override IEnumerable<Namespace> Analyze(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("AnalysisCompilation").AddSyntaxTrees(tree);
            var semanticModel = compilation.GetSemanticModel(tree);

            var analyzedNamespaces = new List<Namespace>();
            var namespaces = tree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            foreach (var n in namespaces)
            {
                var ns = semanticModel.GetDeclaredSymbol(n) as INamespaceSymbol;
                if (ns == null)
                    continue;

                var analyzedClasses = new List<Class>();
                var classes = n.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var c in classes)
                {
                    var cs = semanticModel.GetDeclaredSymbol(c) as INamedTypeSymbol;
                    if (cs == null)
                        continue;

                    var classMembers = cs.GetMembers();

                    var analyzedFields = new List<Field>();
                    var fields = classMembers.Where(x => x.Kind == SymbolKind.Field);
                    foreach (IFieldSymbol f in fields)
                    {
                        analyzedFields.Add(new Field
                        {
                            Name = f.Name,
                            FullyQualifiedIdentifier = f.ToDisplayString(),
                        });
                    }

                    var analyzedMethods = new List<Method>();
                    var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    foreach (var m in methods)
                    {
                        var ms = semanticModel.GetDeclaredSymbol(m) as IMethodSymbol;
                        if (ms == null)
                            continue;

                        var referencedFields = new List<Field>(
                            analyzedFields
                                .Where(f => m.DescendantNodes()
                                    .OfType<IdentifierNameSyntax>()
                                    .Select(x => semanticModel.GetSymbolInfo(x))
                                    .Where(x => x.Symbol != null)
                                    .Select(x => x.Symbol.ToDisplayString())
                                    .Contains(f.FullyQualifiedIdentifier)));

                        analyzedMethods.Add(new Method
                        {
                            Name = ms.Name,
                            FullyQualifiedIdentifier = ms.ToDisplayString(),
                            LineCount = m.Body.DescendantNodes().OfType<StatementSyntax>().Count(),
                            ExitPoints = m.Body.DescendantNodes().OfType<ReturnStatementSyntax>().Count(),
                            Complexity = CalculateComplexity(m.Body),
                            ReferencedFields = referencedFields
                        });
                    }

                    analyzedClasses.Add(new Class
                    {
                        Name = cs.Name,
                        FullyQualifiedIdentifier = cs.ToDisplayString(),
                        Methods = analyzedMethods,
                        Fields = analyzedFields,
                        BaseClass = cs.BaseType != null ? new Class { FullyQualifiedIdentifier = cs.BaseType.ToDisplayString() } : null
                    });
                }

                analyzedNamespaces.Add(new Namespace
                {
                    Name = ns.Name,
                    FullyQualifiedIdentifier = ns.ToDisplayString(),
                    Classes = analyzedClasses
                });
            }

            return analyzedNamespaces;
        }

        private int CalculateComplexity(BlockSyntax block)
        {
            // Based on: http://www.aivosto.com/project/help/pm-complexity.html

            var elementsKind = new[]
			{  
                // Common decisions keywords
                SyntaxKind.IfStatement,
                SyntaxKind.SwitchStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.TryStatement,

                // Logical operations keywords
				SyntaxKind.LogicalAndExpression, 
				SyntaxKind.LogicalOrExpression, 
				SyntaxKind.LogicalNotExpression,
            };
            
            var decisions = block.DescendantNodes().Where(x => elementsKind.Contains(x.Kind())).Count();
            return decisions + 1;
        }
    }
}
