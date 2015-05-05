using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Model
{
    public class ElementParameter
    {

        public static IDictionary<string, object> ListParameters<TElement>(string prefix, IEnumerable<TElement> elements)
            where TElement : IElement, new()
        {
            var element = elements.FirstOrDefault();
            if (element == null)
                element = new TElement();

            var result = new Dictionary<string, object>();
            foreach (var p in element.Parameters)
            {
                var dataList = new List<object>();
                foreach (var e in elements)
                { 
                    if (e.Parameters[p.Key].GetType().IsArray)
                    {
                        var a = (Array)e.Parameters[p.Key];
                        foreach (var v in a)
                            dataList.Add(v);
                    }
                    else
                        dataList.Add(e.Parameters[p.Key]);
                }

                var key = String.Format("{0}.{1}", prefix, p.Key);
                result.Add(key, dataList.ToArray());
            }

            return result;
        }
    }
}
