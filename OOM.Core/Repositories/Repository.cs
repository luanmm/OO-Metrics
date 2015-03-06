using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OOM.Core.Security;
using System.IO;
using System.Reflection;
using System.Threading;

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

        public abstract IEnumerable<RepositoryNode> ListRevisionNodes(string revision);

        public abstract Stream GetNodeContent(RepositoryNode node);

        public bool Delete()
        {
            try
            {
                EmptyRepository();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void EmptyRepository()
        {
            var tries = 0;
            do
            {
                try
                {
                    DeleteDirectoryRecursively(LocalPath);
                    break;
                }
                catch (Exception)
                {
                    if (tries++ == 10)
                        break;
                }

                Thread.Sleep(800);
            } while (Directory.Exists(LocalPath));
        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Privates

        private void DeleteDirectoryRecursively(string baseDirectory)
        {
            var files = Directory.GetFiles(baseDirectory);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            var directories = Directory.GetDirectories(baseDirectory);
            foreach (var directory in directories)
                DeleteDirectoryRecursively(directory);

            Directory.Delete(baseDirectory, false);
        }

        #endregion
    }
}
