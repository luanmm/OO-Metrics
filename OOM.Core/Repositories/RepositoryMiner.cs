using OOM.Model;
using OOM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryMiner : IDisposable
    {
        private ProjectRepository _projectRepository;
        private Project _project;
        private Repository _repository;

        public RepositoryMiner(Project project)
        {
            _projectRepository = new ProjectRepository();
            _project = _projectRepository.GetByUri(project.URI);
            if (_project == null)
                throw new Exception("The specified project could not be found in the database or is invalid.");

            _repository = RepositoryFactory.CreateRepository(_project.RepositoryProtocol, new RepositoryConfiguration(_project.URI, _project.User, _project.Password));
        }

        public void StartMining()
        {
            string lastRevisionRID = null;

            var lastRevision = _project.Revisions.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (lastRevision != null)
                lastRevisionRID = lastRevision.RID;

            var revisionRepository = new RevisionRepository();
            var revisions = _repository.ListRevisions(lastRevisionRID);
            foreach (var revision in revisions) 
            {
                revisionRepository.Create(new Revision
                {
                    RID = revision.RID,
                    Message = revision.Message,
                    Author = revision.Author,
                    CreatedAt = revision.CreatedAt,
                    ProjectId = _project.Id
                });
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
