using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        TEntity GetById(TKey id);
        TKey Create(TEntity entity);
        TKey Update(TEntity entity);
        bool Delete(TKey id);
    }
}
