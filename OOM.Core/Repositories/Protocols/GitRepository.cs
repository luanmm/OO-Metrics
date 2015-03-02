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

        public override IEnumerable<RepositoryNode> ListRevisionNodes(string revision)
        {
            var commit = _repository.Commits.FirstOrDefault(c => c.Sha == revision);
            if (commit == null)
                throw new Exception("This revision wasn't found in the specified repository.");

            var nodes = new List<RepositoryNode>();
            ListRevisionFiles(revision, commit.Tree, nodes);

            return nodes;
        }

        public override Stream GetNodeContent(RepositoryNode node)
        {
            var commit = _repository.Lookup<Commit>(node.Revision);
            var treeEntry = commit[node.Filename];

            if (treeEntry.TargetType == TreeEntryTargetType.Blob)
                return (treeEntry.Target as Blob).GetContentStream();

            return null;
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

        private void ListRevisionFiles(string revision, Tree tree, List<RepositoryNode> nodes)
        {
            foreach (var node in tree)
            {
                switch (node.TargetType)
                {
                    case TreeEntryTargetType.Tree:
                        ListRevisionFiles(revision, node.Target as Tree, nodes);
                        break;
                    case TreeEntryTargetType.Blob:
                        nodes.Add(new RepositoryNode
                        {
                            Filename = node.Path,
                            Revision = revision
                        });
                        break;
                }
            }
        }

        #endregion
    }
}
