using OOM.Core.Analyzers.CSharp;
using OOM.Core.Analyzers.Java;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OOM.Core.Analyzers
{
    public static class CodeAnalyzerFactory
    {
        public static CodeAnalyzer CreateCodeAnalyzer(string filename)
        {
            return CreateCodeAnalyzer(filename, new CodeAnalyzerConfiguration());
        }

        public static CodeAnalyzer CreateCodeAnalyzer(string filename, CodeAnalyzerConfiguration configuration)
        {
            if (Regex.IsMatch(filename, ".[cC]{1}[sS]{1}$"))
                return new CSharpCodeAnalyzer(configuration);

            if (Regex.IsMatch(filename, ".[jJ]{1}[aA]{1}[vV]{1}[aA]{1}$"))
                return new JavaCodeAnalyzer(configuration);

            return null;
        }
    }
}
