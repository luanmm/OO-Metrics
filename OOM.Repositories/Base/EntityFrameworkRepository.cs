using OOM.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Repositories
{
    public class EntityFrameworkRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        #region Public Members

        public virtual TEntity GetById(TKey id)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<TEntity>().Find(id);
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<TEntity>().ToList();
            }
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<TEntity>().Where(predicate).ToList();
            }
        }

        public virtual PagedResult<TEntity> Find<TResult>(IQueryable<TEntity> query, int pageNum, int pageSize,
                Expression<Func<TEntity, TResult>> orderByProperty, bool isAscendingOrder, out int rowsCount)
        {
            if (pageSize <= 0) pageSize = 20;

            rowsCount = query.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;

            query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            return new PagedResult<TEntity>(query.Skip(excludedRows).Take(pageSize), rowsCount);
        }

        public virtual TKey Create(TEntity entity)
        {
            this.Insert<TEntity>(entity);

            return entity.Id;
        }

        public virtual TKey Update(TEntity entity)
        {
            this.Update<TEntity>(entity);

            return entity.Id;
        }

        public virtual bool Delete(TKey id)
        {
            this.Delete<TEntity>(id);

            return true;
        }

        #endregion

        #region Protected Members

        protected void Insert<E>(E entity)
            where E : class, IEntity<TKey>
        {
            using (var context = new OOMetricsContext())
            {
                context.Set<E>().Add(entity);
                context.SaveChanges();
            }
        }

        protected void Update<E>(E entity)
            where E : class, IEntity<TKey>
        {
            using (var context = new OOMetricsContext())
            {
                context.Set<E>().Attach(entity);
                context.Entry<E>(entity).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        protected void Delete<E>(TKey id)
            where E : class, IEntity<TKey>
        {
            using (var context = new OOMetricsContext())
            {
                context.Set<E>().Remove(context.Set<E>().Find(id));
                context.SaveChanges();
            }
        }

        public IEnumerable<E> Find<E>()
            where E : class, IEntity<TKey>
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<E>().ToList();
            }
        }

        public IEnumerable<E> Find<E>(Expression<Func<E, bool>> predicate)
            where E : class, IEntity<TKey>
        {
            using (var context = new OOMetricsContext())
            {
                return context.Set<E>().Where(predicate).ToList();
            }
        }

        #endregion
    }
}
