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

        public override IEnumerable<Namespace> Analyze(string mainFile)
        {
            var analyzedNamespaces = new List<Namespace>();
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(mainFile).Result;
            foreach (var project in solution.Projects)
            {
                if (!project.SupportsCompilation)
                    continue;

                var compilation = project.GetCompilationAsync().Result;
                if (!compilation.Language.Equals(LanguageNames.CSharp))
                    continue;
                
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (!syntaxTree.HasCompilationUnitRoot)
                        continue;

                    var model = compilation.GetSemanticModel(syntaxTree);

                    var namespaces = syntaxTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                    foreach (var n in namespaces)
                    {
                        var ns = (INamespaceSymbol)model.GetDeclaredSymbol(n);
                        if (ns == null)
                            continue;

                        var analyzedClasses = new List<Class>();
                        var classes = n.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var c in classes)
                        {
                            var cs = (ISymbol)model.GetDeclaredSymbol(c);
                            if (cs == null)
                                continue;

                            var analyzedFields = new List<Field>();
                            var fields = c.DescendantNodes().OfType<FieldDeclarationSyntax>();
                            foreach (var f in fields)
                            {
                                var fs = (IFieldSymbol)model.GetDeclaredSymbol(f);
                                if (fs == null)
                                    continue;

                                analyzedFields.Add(new Field
                                {
                                    Name = fs.Name,
                                    FullyQualifiedIdentifier = fs.ToDisplayString(),
                                });
                            }

                            var analyzedMethods = new List<Method>();
                            var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>();
                            foreach (var m in methods)
                            {
                                var ms = (IMethodSymbol)model.GetDeclaredSymbol(m);
                                if (ms == null)
                                    continue;

                                var mr = m.ChildNodes().OfType<ExpressionSyntax>();
                                foreach (var mref in mr)
                                {
                                    Console.WriteLine(mref.ToString());
                                }

                                /*
                                var referencedFields = new List<Field>();
                                var methodBody = m.Body;
                                if (methodBody != null)
                                {
                                    var accessVarsDecl = from aAccessVarsDecl in methodBody.ChildNodes().OfType<LocalDeclarationStatementSyntax>() select aAccessVarsDecl;
                                    foreach (var ldss in accessVarsDecl)
                                    {
                                        referencedFields.Add(new Field
                                        {
                                            Name = ldss.
                                        });

                                        Method tempMethod = TransverseAccessVars(ldss);
                                        retMethod.AccessedVariables.AddRange(tempMethod.AccessedVariables);
                                        retMethod.InvokedMethods.AddRange(tempMethod.InvokedMethods);
                                    }
                                }
                                */

                                var analyzedMethod = new Method
                                {
                                    Name = ms.Name,
                                    FullyQualifiedIdentifier = ms.ToDisplayString(),
                                    LineCount = m.Body.Statements.Count,
                                    ExitPoints = 0,
                                    ReferencedFields = new List<Field>()
                                };

                                /*
                                var referencedMethods = m.Body.ChildNodes().OfType<ExpressionStatementSyntax>();
                                foreach (var referencedMethod in referencedMethods)
                                {
                                    var symbolInfo = SemanticModel.GetSymbolInfo(referencedMethod);
                                    string ns = symbolInfo.Symbol. //.ContainingNamespace.ToDisplayString();

                                    analyzedMethod.Attributes.Add(new Attribute
                                    {
                                        Identifier = referencedMethod.
                                    });
                                }
                                */

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
                }
            }

            return analyzedNamespaces;
        }
    }
}
