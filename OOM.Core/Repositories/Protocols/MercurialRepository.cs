using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurial;
using System.IO;
using System.Reflection;

namespace OOM.Core.Repositories.Protocols
{
    public class MercurialRepository : Repository
    {
        private RepositoryConfiguration _repositoryConfig;
        private Mercurial.Repository _repository;

        public MercurialRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            var clientPath = new Uri(String.Format("{0}\\App_Data\\Binaries\\Mercurial\\hg.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase))).LocalPath;

            Mercurial.Client.SetClientPath(clientPath);
            Mercurial.Client.SetClientPath(clientPath);

            if (!Mercurial.Client.CouldLocateClient)
                throw new DirectoryNotFoundException("The Mercurial client wasn't valid.");

            _repositoryConfig = configuration;
            _repository = InitializeRepository();
        }

        public override bool Update()
        {
            try
            {
                _repository.Update(new UpdateCommand { 
                    Clean = true
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Privates

        private Mercurial.Repository InitializeRepository()
        {
            var repository = new Mercurial.Repository(LocalPath);
            if (!Directory.Exists(String.Format("{0}\\.hg", LocalPath)))
            {
                base.EmptyRepository();
                repository.Clone(new CloneCommand
                {
                    Update = false,
                    Source = _repositoryConfig.RemotePath
                });
            }

            return repository;
        }

        #endregion
    }
}
