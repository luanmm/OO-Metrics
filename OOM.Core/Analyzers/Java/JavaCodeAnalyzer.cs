using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Analyzers.Java
{
    public class JavaCodeAnalyzer : CodeAnalyzer
    {
        public JavaCodeAnalyzer(CodeAnalyzerConfiguration configuration)
            : base(configuration)
        {
            
        }

        public override AnalyzedCode Analyze(string code)
        {
            throw new NotImplementedException();
        }
    }
}
