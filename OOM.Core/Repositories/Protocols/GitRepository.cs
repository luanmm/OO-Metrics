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
        private LibGit2Sharp.Repository _repository;

        public GitRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            _repository = InitializeRepository();
        }

        public override IEnumerable<RepositoryRevision> ListRevisions(string fromCommit = null)
        {
            IEnumerable<Commit> commitLog = _repository.Commits;
            if (!String.IsNullOrWhiteSpace(fromCommit))
            {
                var referenceCommit = _repository.Commits.FirstOrDefault(c => c.Sha == fromCommit);
                commitLog = _repository.Commits.Where(c => c.Committer.When.CompareTo(referenceCommit.Committer.When) > 0);
            }

            var revisionList = new List<RepositoryRevision>();
            foreach (var commit in commitLog)
            {
                revisionList.Add(new RepositoryRevision
                {
                    RID = commit.Sha,
                    Message = commit.Message,
                    Author = String.IsNullOrWhiteSpace(commit.Author.Email) ? commit.Author.Name : String.Format("{0} <{1}>", commit.Author.Name, commit.Author.Email),
                    CreatedAt = commit.Committer.When.DateTime
                });
            }

            return revisionList.OrderBy(r => r.CreatedAt);
        }

        public override void Dispose()
        {
            _repository.Dispose();
        }

        #region Privates

        private LibGit2Sharp.Repository InitializeRepository()
        {
            base.EmptyRepository();
            LibGit2Sharp.Repository.Clone(Configuration.RemotePath, LocalPath, new CloneOptions
            {
                CredentialsProvider = PrivateRepositoryCredentials,
                IsBare = true
            });

            return new LibGit2Sharp.Repository(LocalPath);
        }

        private LibGit2Sharp.Credentials PrivateRepositoryCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types) 
        {
            return new UsernamePasswordCredentials { Username = Configuration.User, Password = Configuration.Password };
        }

        #endregion
    }
}
