using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace OOM.Core.Repositories.Protocols
{
    public class SubversionRepository : Repository, IDisposable
    {
        private SvnTarget _target;
        private SvnClient _client;

        public SubversionRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            //_target = SvnTarget.FromString(configuration);
            //_client = new SvnClient();
        }

        public override bool Update()
        {
            return true;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
