using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace OOM.Core.Repositories.Protocols
{
    public class GitRepository : Repository
    {
        private RepositoryConfiguration _repositoryConfig;
        private LibGit2Sharp.Repository _repository;

        public GitRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            _repositoryConfig = configuration;
            _repository = InitializeRepository();
        }

        public override bool Update()
        {
            try
            {
                _repository.Reset(ResetMode.Hard);
                _repository.Network.Pull(new Signature("OO-Metrics", "mail@oo-metrics.project.com", new DateTimeOffset(2011, 06, 16, 10, 58, 27, TimeSpan.FromHours(2))), new PullOptions
                {
                    MergeOptions = new MergeOptions
                    {
                        FastForwardStrategy = FastForwardStrategy.FastForwardOnly,
                        FileConflictStrategy = CheckoutFileConflictStrategy.Theirs
                    },
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = PrivateRepositoryCredentials,
                        TagFetchMode = TagFetchMode.None
                    }
                });
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public override void Dispose()
        {
            _repository.Dispose();
        }

        #region Privates

        private LibGit2Sharp.Repository InitializeRepository()
        {
            if (!LibGit2Sharp.Repository.IsValid(LocalPath))
            {
                base.EmptyRepository();
                LibGit2Sharp.Repository.Clone(_repositoryConfig.RemotePath, LocalPath, new CloneOptions
                {
                    CredentialsProvider = PrivateRepositoryCredentials,
                    Checkout = true
                });
            }

            return new LibGit2Sharp.Repository(LocalPath);
        }

        private LibGit2Sharp.Credentials PrivateRepositoryCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types) 
        {
            return new UsernamePasswordCredentials { Username = _repositoryConfig.User, Password = _repositoryConfig.Password };
        }

        #endregion
    }
}
