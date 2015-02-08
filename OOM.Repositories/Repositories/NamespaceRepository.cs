using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOM.Model;

namespace OOM.Repositories
{
    public class NamespaceRepository : EntityFrameworkRepository<Namespace, int>
    {
        public Namespace GetByIdentifier(int revisionId, string identifier)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<Namespace>().FirstOrDefault(n => n.RevisionId == revisionId && n.Identifier == identifier);
            }
        }
    }
}