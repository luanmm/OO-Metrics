using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Repositories
{
    public class PagedResult<TEntity>
    {
        private IEnumerable<TEntity> _items;
        public IEnumerable<TEntity> Items { get { return _items; } }

        private int _totalCount;
        public int TotalCount { get { return _totalCount; } }

        public PagedResult(IEnumerable<TEntity> items, int totalCount)
        {
            _items = items;
            _totalCount = totalCount;
        }
    }
}
