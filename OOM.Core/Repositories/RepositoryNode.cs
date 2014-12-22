using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryNode
    {
        /*
        public string BasePath { get; }
        public Uri BaseUri { get; }

        //public SvnDirEntry Entry { get; }
        public string Author { get; }
        public long FileSize { get; }
        public bool HasProperties { get; }
        public RepositoryNodeKind NodeKind { get; }
        public long Revision { get; }
        public DateTime Time { get; }

        public string Name { get; }
        public string Path { get; }
        public Uri RepositoryRoot { get; }
        public Uri Uri { get; }*/
    }

    public enum RepositoryNodeKind
    {
        None = 0,
        File = 1,
        Directory = 2,
        Unknown = 3,
    }
}
