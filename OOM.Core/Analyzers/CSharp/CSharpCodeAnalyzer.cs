using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OOM.Model;
using System;
using System.Collections.Generic;
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

        public override AnalyzedCode Analyze(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var analyzedNamespaces = new List<AnalyzedNamespace>();
            var namespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            foreach (var n in namespaces)
            {
                var analyzedClasses = new List<AnalyzedClass>();
                var classes = n.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var c in classes)
                {
                    var analyzedAttributes = new List<AnalyzedAttribute>();
                    var attributes = c.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                    foreach (var a in attributes)
                    {
                        analyzedAttributes.Add(new AnalyzedAttribute
                        {
                            Identifier = a.Identifier.ToString()
                        });
                    }

                    var analyzedMethods = new List<AnalyzedMethod>();
                    var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    foreach (var m in methods)
                    {
                        analyzedMethods.Add(new AnalyzedMethod
                        {
                            Identifier = m.Identifier.ToString(),
                            LineCount = m.Body.Statements.Count,
                            //Attributes = m.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Select(x => new AnalyzedAttribute { Identifier = x.GetText().ToString() }).ToList()
                        });
                    }

                    analyzedClasses.Add(new AnalyzedClass
                    { 
                        Identifier = c.Identifier.ToString(),
                        Attributes = analyzedAttributes,
                        Methods = analyzedMethods
                    });
                }

                analyzedNamespaces.Add(new AnalyzedNamespace
                {
                    Identifier = n.Name.ToString(), 
                    Classes = analyzedClasses
                });
            }

            return new AnalyzedCode
            { 
                Namespaces = analyzedNamespaces
            };
        }
    }
}
