using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Analyzers.CSharp
{
    public class CSharpCodeAnalyzer
    {
        public void Analyze(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var c in classes) 
            {
                var methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var m in methods)
                {
                    var linesOfCode = m.DescendantNodes().OfType<StatementSyntax>().ToList().Count;
                    Console.WriteLine(String.Format("Class: {0}, Method: {1}, Lines of code: {2}", c.Identifier, m.Identifier, linesOfCode));
                }
            }
        }
    }
}
