using OOM.Core.Repositories.Protocols;
using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public static class RepositoryFactory
    {
        public static Repository CreateRepository(RepositoryProtocol protocol, RepositoryConfiguration configuration)
        {
            Repository repository = null;
            switch (protocol)
            {
                case RepositoryProtocol.Git:
                    repository = new GitRepository(configuration);
                    break;
                case RepositoryProtocol.Mercurial:
                    repository = new MercurialRepository(configuration);
                    break;
                case RepositoryProtocol.Subversion:
                    repository = new SubversionRepository(configuration);
                    break;
            }
            return repository;
        }
    }
}
