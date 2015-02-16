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

        public override IEnumerable<RepositoryNode> ListRevisionTree(string revision)
        {
            var nodes = new List<RepositoryNode>();
            foreach (var node in _repository.Lookup<Commit>(revision).Tree)
            {
                var nodeType = NodeType.Unknown;
                switch (node.TargetType)
                {
                    case TreeEntryTargetType.Tree:
                        nodeType = NodeType.Directory;
                        break;
                    case TreeEntryTargetType.Blob:
                        nodeType = NodeType.File;
                        break;
                }

                nodes.Add(new RepositoryNode
                {
                    Name = node.Name,
                    Path = node.Path,
                    Type = nodeType,
                    Revision = revision,
                    RepositoryObject = node
                });
            }

            return nodes;
        }

        public override IEnumerable<RepositoryNode> ListNodeTree(RepositoryNode rootNode)
        {
            try {
                var nodes = new List<RepositoryNode>();
                var tree = ((rootNode.RepositoryObject as TreeEntry).Target as Tree);
                foreach (var node in tree)
                {
                    var nodeType = NodeType.Unknown;
                    switch (node.TargetType)
                    {
                        case TreeEntryTargetType.Tree:
                            nodeType = NodeType.Directory;
                            break;
                        case TreeEntryTargetType.Blob:
                            nodeType = NodeType.File;
                            break;
                    }

                    nodes.Add(new RepositoryNode
                    {
                        Name = node.Name,
                        Path = node.Path,
                        Type = nodeType,
                        Revision = rootNode.Revision,
                        RepositoryObject = node
                    });
                }

                return nodes;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("This node object is not from a Git repository.");
            }
        }

        public override Stream GetNodeContent(RepositoryNode node)
        {
            var commit = _repository.Lookup<Commit>(node.Revision);
            var treeEntry = commit[node.Path];

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

        #endregion
    }
}
