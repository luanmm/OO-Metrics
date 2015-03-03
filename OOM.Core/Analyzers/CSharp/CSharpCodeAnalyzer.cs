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
                    var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>(); //classMembers.Where(x => x.Kind == SymbolKind.Method);
                    foreach (var m in methods)
                    {
                        var ms = semanticModel.GetDeclaredSymbol(m) as IMethodSymbol;
                        if (ms == null)
                            continue;

                        var referencedFields = new List<Field>(
                            analyzedFields
                                .Where(f => m.Body.DescendantNodes()
                                    .OfType<IdentifierNameSyntax>()
                                    .Select(x => semanticModel.GetSymbolInfo(x).Symbol.ToDisplayString())
                                    .Contains(f.FullyQualifiedIdentifier)));

                        var analyzedMethod = new Method
                        {
                            Name = ms.Name,
                            FullyQualifiedIdentifier = ms.ToDisplayString(),
                            LineCount = m.Body.Statements.Count,
                            ExitPoints = 0,
                            ReferencedFields = referencedFields
                        };

                        analyzedMethods.Add(analyzedMethod);
                    }

                    analyzedClasses.Add(new Class
                    {
                        Name = cs.Name,
                        FullyQualifiedIdentifier = cs.ToDisplayString(),
                        Methods = analyzedMethods,
                        Fields = analyzedFields
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
    }
}
