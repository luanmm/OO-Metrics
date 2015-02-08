using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOM.Model;

namespace OOM.Repositories
{
    public class ClassRepository : EntityFrameworkRepository<Class, int>
    {
        public Class GetByIdentifier(int namespaceId, string identifier)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<Class>().FirstOrDefault(c => c.NamespaceId == namespaceId && c.Identifier == identifier);
            }
        }
    }
}