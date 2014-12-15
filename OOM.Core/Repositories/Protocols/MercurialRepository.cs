using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurial;

namespace OOM.Core.Repositories.Protocols
{
    public class MercurialRepository : Repository
    {
        private Mercurial.Repository _repository;

        public MercurialRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            //_repository = new Mercurial.Repository(LocalPath);
        }

        public override bool Update()
        {
            return true;
        }
    }
}
