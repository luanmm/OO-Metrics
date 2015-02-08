using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Analyzers
{
    public abstract class CodeAnalyzer : IDisposable
    {
        #region Properties

        protected CodeAnalyzerConfiguration Configuration { get; private set; }

        #endregion

        #region Ctor

        public CodeAnalyzer(CodeAnalyzerConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Methods

        public abstract AnalyzedCode Analyze(string code);

        #endregion

        #region Privates

        public virtual void Dispose()
        {

        }

        #endregion
    }
}
