using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Model
{
    [Flags]
    public enum EncapsulationTypes : int
    {
        Private = 1,
        Protected = 2,
        Public = 4,
    }
}
