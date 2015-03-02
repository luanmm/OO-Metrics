using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using OOM.Model;
using System.IO;

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

        public override IEnumerable<RepositoryRevision> ListRevisions(string fromRevision = null)
        {
            IEnumerable<Commit> revisionLog = _repository.Commits;
            if (!String.IsNullOrWhiteSpace(fromRevision))
            {
                var referenceRevision = _repository.Commits.FirstOrDefault(c => c.Sha == fromRevision);
                revisionLog = _repository.Commits.Where(c => c.Committer.When.CompareTo(referenceRevision.Committer.When) > 0);
            }

            var revisionList = new List<RepositoryRevision>();
            foreach (var revision in revisionLog)
            {
                revisionList.Add(new RepositoryRevision
                {
                    RID = revision.Sha,
                    Message = revision.Message,
                    Author = String.IsNullOrWhiteSpace(revision.Author.Email) ? revision.Author.Name : String.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email),
                    CreatedAt = revision.Committer.When.DateTime
                });
            }

            return revisionList.OrderBy(r => r.CreatedAt);
        }

        public override IEnumerable<string> ListRevisionFiles(string revision)
        {
            var commit = _repository.Commits.FirstOrDefault(c => c.Sha == revision);
            if (commit == null)
                throw new Exception("This revision wasn't found in the specified repository.");

            _repository.Checkout(commit);

            var files = _repository.Index.Select(x => String.Format("{0}{1}", LocalPath, x.Path));
            foreach (var file in files) 
            {
                if (!File.Exists(file))
                    throw new Exception("Repository integrity error: the index doesn't corresponds the physical content.");
            }

            return files;
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
                CredentialsProvider = PrivateRepositoryCredentials
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
