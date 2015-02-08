using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOM.Model
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
