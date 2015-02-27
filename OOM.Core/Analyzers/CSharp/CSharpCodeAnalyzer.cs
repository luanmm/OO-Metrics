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
        private MetadataReference mscorlib;
        private MetadataReference Mscorlib
        {
            get
            {
                if (mscorlib == null)
                {
                    mscorlib = MetadataReference.CreateFromAssembly(typeof(object).Assembly);
                }

                return mscorlib;
            }
        }

        public CSharpCodeAnalyzer(CodeAnalyzerConfiguration configuration)
            : base(configuration)
        {

        }

        public override AnalyzedCode Analyze(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var compilation = CSharpCompilation.Create("AnalysisCompilation",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);

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
                            Identifier = a.Identifier.ValueText
                        });
                    }

                    var analyzedMethods = new List<AnalyzedMethod>();
                    var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    foreach (var m in methods)
                    {
                        var ms = (IMethodSymbol)model.GetDeclaredSymbol(m);

                        var methodDefinitionType = ElementDefinitionType.Defines;
                        if (ms.IsOverride)
                            methodDefinitionType = ElementDefinitionType.Overrides;
                        else if (ms.IsExtensionMethod)
                            methodDefinitionType = ElementDefinitionType.Extends;

                        var methodVisibility = ElementVisibility.Private;
                        if (ms.DeclaredAccessibility == Accessibility.Public)
                            methodVisibility = ElementVisibility.Public;
                        else if (ms.DeclaredAccessibility == Accessibility.Protected)
                            methodVisibility = ElementVisibility.Protected;

                        var analyzedMethod = new AnalyzedMethod
                        {
                            Identifier = m.Identifier.ValueText,
                            Scope = ms.IsStatic ? ElementScope.Class : ElementScope.Instance,
                            Abstractness = ms.IsAbstract ? ElementAbstractness.Abstract : ElementAbstractness.Concrete,
                            DefinitionType = methodDefinitionType, 
                            Visibility = methodVisibility,
                            LineCount = m.Body.Statements.Count,
                            Attributes = new List<AnalyzedAttribute>()
                        };

                        /*
                        var referencedMethods = m.Body.ChildNodes().OfType<ExpressionStatementSyntax>();
                        foreach (var referencedMethod in referencedMethods)
                        {
                            var symbolInfo = SemanticModel.GetSymbolInfo(referencedMethod);
                            string ns = symbolInfo.Symbol. //.ContainingNamespace.ToDisplayString();

                            analyzedMethod.Attributes.Add(new AnalyzedAttribute
                            {
                                Identifier = referencedMethod.
                            });
                        }
                        */
                        analyzedMethods.Add(analyzedMethod);

                        var x = m.SyntaxTree.Length;
                        //m.Body.ChildNodes().OfType<ExpressionStatementSyntax>()
                    }

                    analyzedClasses.Add(new AnalyzedClass
                    { 
                        Identifier = c.Identifier.ValueText,
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
