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
    public class RevisionsController : Controller
    {
        private OOMetricsContext _db = new OOMetricsContext();

        // GET: /Revisions/List/5
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(project.Revisions);
        }

        // GET: /Revisions/Details/5
        public ActionResult Details(int id)
        {
            var revision = _db.Revisions.Find(id);
            if (revision == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(revision);
        }

        // GET: /Revisions/Graph/5
        public JsonResult Graph(int id)
        {
            var nodes = new List<object>();
            var links = new List<object>();

            var revision = _db.Revisions.Find(id);
            var revisionNodeIndex = nodes.Count;
            nodes.Add(new
            {
                name = "This revision.",
                group = 1
            });

            foreach (var ns in revision.Namespaces)
            {
                var namespaceNodeIndex = nodes.Count;
                nodes.Add(new
                {
                    name = ns.FullyQualifiedIdentifier,
                    group = 2,
                    url = String.Format("{0}?elementType=namespace&elementId={1}&revisionId={2}", Url.Action("History", "Metrics"), ns.Id, revision.Id)
                });
                links.Add(new
                {
                    source = revisionNodeIndex,
                    target = namespaceNodeIndex
                });

                foreach (var c in ns.Classes)
                {
                    var classNodeIndex = nodes.Count;
                    nodes.Add(new
                    {
                        name = c.Name,
                        group = 3,
                        url = String.Format("{0}?elementType=class&elementId={1}&revisionId={2}", Url.Action("History", "Metrics"), c.Id, revision.Id)
                    });
                    links.Add(new
                    {
                        source = namespaceNodeIndex,
                        target = classNodeIndex
                    });

                    foreach (var f in c.Fields)
                    {
                        var fieldNodeIndex = nodes.Count;
                        nodes.Add(new
                        {
                            name = f.Name,
                            group = 4,
                            url = String.Format("{0}?elementType=field&elementId={1}&revisionId={2}", Url.Action("History", "Metrics"), f.Id, revision.Id)
                        });
                        links.Add(new
                        {
                            source = classNodeIndex,
                            target = fieldNodeIndex
                        });
                    }

                    foreach (var m in c.Methods)
                    {
                        var methodNodeIndex = nodes.Count;
                        nodes.Add(new
                        {
                            name = m.Name,
                            group = 5,
                            url = String.Format("{0}?elementType=method&elementId={1}&revisionId={2}", Url.Action("History", "Metrics"), m.Id, revision.Id)
                        });
                        links.Add(new
                        {
                            source = classNodeIndex,
                            target = methodNodeIndex
                        });
                    }
                }
            }

            return Json(new
            {
                nodes = nodes,
                links = links
            }, JsonRequestBehavior.AllowGet);
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