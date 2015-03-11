using OOM.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OOM.Web.Controllers
{
    public class ElementsController : Controller
    {
        private OOMetricsContext _db = new OOMetricsContext();

        // GET: /Elements/Namespace/5
        public ActionResult Namespace(int id)
        {
            var n = _db.Namespaces.Find(id);
            if (n == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(n);
        }

        // GET: /Elements/Class/5
        public ActionResult Class(int id)
        {
            var c = _db.Classes.Find(id);
            if (c == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(c);
        }

        // GET: /Elements/Method/5
        public ActionResult Method(int id)
        {
            var m = _db.Methods.Find(id);
            if (m == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(m);
        }

        // GET: /Elements/Field/5
        public ActionResult Field(int id)
        {
            var f = _db.Fields.Find(id);
            if (f == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}