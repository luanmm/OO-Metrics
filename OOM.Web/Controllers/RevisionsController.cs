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
                name = String.Format("[{0} @ {1:dd/MM/yy HH:mm:ss}] {2}", revision.Author, revision.CreatedAt, revision.Message),
                group = 1
            });

            foreach (var ns in revision.Namespaces)
            {
                var namespaceNodeIndex = nodes.Count;
                nodes.Add(new
                {
                    name = ns.Name,
                    group = 2
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
                        group = 3
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
                            group = 4
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
                            group = 5
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