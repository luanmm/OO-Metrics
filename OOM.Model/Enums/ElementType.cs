using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOM.Model
{
    public enum ElementType : int
    {
        Namespace = 1,
        Class = 2,
        Field = 3,
        Method = 4,
    }

    public static class ElementTypeExtension
    {
        public static Type ToElement(this ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Namespace:
                    return typeof(Namespace);
                case ElementType.Class:
                    return typeof(Class);
                case ElementType.Field:
                    return typeof(Field);
                case ElementType.Method:
                    return typeof(Method);
                default:
                    return null;
            }
        }
    }
}
