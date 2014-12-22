using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryMiner
    {
        private Repository _repository;

        public RepositoryMiner(Repository repository)
        {
            _repository = repository;
        }

        public void StartMining()
        {
            // TODO: Implement mining routines

            _repository.Update();
            // ...
        }
    }
}
