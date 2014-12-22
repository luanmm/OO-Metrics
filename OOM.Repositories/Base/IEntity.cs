using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOM.Repositories
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
