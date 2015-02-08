using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Analyzers
{
    public class AnalyzedCode
    {
        public IEnumerable<AnalyzedNamespace> Namespaces { get; set; }
    }

    public class AnalyzedNamespace
    {
        public string Identifier { get; set; }

        public IEnumerable<AnalyzedClass> Classes { get; set; }
    }

    public class AnalyzedClass
    {
        public string Identifier { get; set; }

        public string BaseClassIdentifier { get; set; }

        public ElementAbstractness Abstractness { get; set; }

        public ElementVisibility Visibility { get; set; }

        public IEnumerable<AnalyzedAttribute> Attributes { get; set; }

        public IEnumerable<AnalyzedMethod> Methods { get; set; }
    }

    public class AnalyzedAttribute
    {
        public string Identifier { get; set; }

        public ElementVisibility Visibility { get; set; }

        public ElementScope Scope { get; set; }
    }

    public class AnalyzedMethod
    {
        public string Identifier { get; set; }

        public ElementAbstractness Abstractness { get; set; }

        public ElementVisibility Visibility { get; set; }

        public ElementScope Scope { get; set; }

        public ElementDefinitionType DefinitionType { get; set; }

        public int LineCount { get; set; }
    }
}
