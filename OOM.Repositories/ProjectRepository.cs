using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOM.Model;

namespace OOM.Repositories
{
    public class ProjectRepository : EntityFrameworkRepository<Project, int>
    {
        public Project GetByUri(string uri)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<Project>().FirstOrDefault(p => p.URI == uri);
            }
        }
    }
}
