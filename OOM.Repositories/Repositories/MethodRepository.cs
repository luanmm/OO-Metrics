using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOM.Model;

namespace OOM.Repositories
{
    public class MethodRepository : EntityFrameworkRepository<Method, int>
    {
        public Method GetByIdentifier(int classId, string identifier)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<Method>().FirstOrDefault(m => m.ClassId == classId && m.Identifier == identifier);
            }
        }
    }
}