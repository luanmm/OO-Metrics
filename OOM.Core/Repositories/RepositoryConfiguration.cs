using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryConfiguration
    {
        public string RemotePath { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }

        public RepositoryConfiguration(string remotePath, string user = null, string password = null)
        {
            RemotePath = remotePath;
            User = user;
            Password = password;
        }
    }
}
