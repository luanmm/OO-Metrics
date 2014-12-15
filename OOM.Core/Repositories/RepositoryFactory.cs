using OOM.Core.Repositories.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public static class RepositoryFactory
    {
        public static Repository CreateRepository(ReporitoryProtocol protocol, RepositoryConfiguration configuration)
        {
            Repository repository = null;
            switch (protocol)
            {
                case ReporitoryProtocol.Git:
                    repository = new GitRepository(configuration);
                    break;
                case ReporitoryProtocol.Mercurial:
                    repository = new MercurialRepository(configuration);
                    break;
                case ReporitoryProtocol.Subversion:
                    repository = new SubversionRepository(configuration);
                    break;
            }
            return repository;
        }
    }

    public enum ReporitoryProtocol
    {
        Git,
        Mercurial,
        Subversion
    }
}
