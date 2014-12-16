using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace OOM.Core.Repositories.Protocols
{
    public class SubversionRepository : Repository
    {
        private SvnClient _client;

        public SubversionRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            _client = new SvnClient();
            if (!_client.CheckOut(new Uri(configuration.RemotePath), LocalPath))
                throw new Exception("An error has ocurred when trying to setup the Subversion repository.");
        }

        public override bool Update()
        {
            return _client.Update(LocalPath);
        }

        public override void Dispose()
        {
            _client.Dispose();
        }
    }
}
