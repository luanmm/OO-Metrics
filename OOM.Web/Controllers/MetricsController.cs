using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OOM.Web.Controllers
{
    public class MetricsController : Controller
    {
        private OOMetricsContext db = new OOMetricsContext();

        // GET: /Metrics/
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}