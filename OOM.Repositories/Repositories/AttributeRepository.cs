using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOM.Model;

namespace OOM.Repositories
{
    public class AttributeRepository : EntityFrameworkRepository<OOM.Model.Attribute, int>
    {
        public OOM.Model.Attribute GetByIdentifier(int classId, string identifier)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<OOM.Model.Attribute>().FirstOrDefault(a => a.ClassId == classId && a.Identifier == identifier);
            }
        }
    }
}