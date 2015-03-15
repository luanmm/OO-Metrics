using OOM.Model;
using OOM.Web.Models;
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

        // GET: /Elements/Details/?revisionId=2&elementType=class&id=5
        public ActionResult Details(ElementType elementType, int elementId, int revisionId)
        {
            var revision = _db.Revisions.Find(revisionId);
            if (revision == null)
                throw new HttpException(400, "Bad request.");

            var element = _db.Set(elementType.ToElement()).Find(elementId) as IElement;
            if (element == null)
                throw new HttpException(400, "Bad request.");

            return View(new ElementDetailsModel
            {
                Element = element,
                Revision = revision
            });
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