using OOM.Web.Extensions;
using System.Web;
using System.Web.Mvc;

namespace OOM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new JsonHandlerAttribute());
        }
    }
}
