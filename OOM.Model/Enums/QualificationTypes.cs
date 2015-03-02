using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Model
{
    [Flags]
    public enum QualificationTypes
    {
        Abstract = 1,
        Extern = 2,
        Override = 4,
        Partial = 8,
        ReadOnly = 16,
        Sealed = 32,
        Final = 64,
        Static = 128,
        Unsafe = 256,
        Virtual = 512,
        Volatile = 1024,
        Const = 2048,
    }
}
