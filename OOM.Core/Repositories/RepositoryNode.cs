using OOM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public NodeType Type { get; set; }
        public string Revision { get; set; }
    }
}
