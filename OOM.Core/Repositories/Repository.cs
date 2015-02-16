using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OOM.Core.Security;
using System.IO;
using System.Reflection;

namespace OOM.Core.Repositories
{
    public abstract class Repository : IDisposable
    {
        #region Properties

        protected string LocalPath { get; private set; }
        protected RepositoryConfiguration Configuration { get; private set; }

        #endregion

        #region Ctor

        public Repository(RepositoryConfiguration configuration)
        {
            var localPath = new Uri(String.Format("{0}\\App_Data\\Repositories\\{1}\\", Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), configuration.RemotePath.ToSHA1())).LocalPath;
            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            LocalPath = localPath;
            Configuration = configuration;
        }

        #endregion

        #region Methods

        public abstract IEnumerable<RepositoryRevision> ListRevisions(string fromRevision = null);

        public abstract IEnumerable<RepositoryNode> ListRevisionTree(string revision);

        public abstract IEnumerable<RepositoryNode> ListNodeTree(RepositoryNode rootNode);

        public abstract Stream GetNodeContent(RepositoryNode node);

        public bool Delete()
        {
            try
            {
                Directory.Delete(LocalPath, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void EmptyRepository()
        {
            Directory.Delete(LocalPath, true);
        }

        #endregion

        #region Privates

        public virtual void Dispose()
        {

        }

        #endregion
    }
}
