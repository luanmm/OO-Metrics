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
    public class ProjectsController : Controller
    {
        private OOMetricsContext _db = new OOMetricsContext();

        // GET: /Projects
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /Projects/List
        public ActionResult List()
        {
            return View(_db.Projects.ToList());
        }

        // GET: /Projects/Details/5
        public ActionResult Details(int? id)
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

            return View(project);
        }

        // GET: /Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Projects/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,RepositoryProtocol,URI,User,Password")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Projects.Add(project);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(project);
        }

        // GET: /Projects/Edit/5
        public ActionResult Edit(int? id)
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

            return View(project);
        }

        // POST: /Projects/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var projectToUpdate = _db.Projects.Find(id);
            if (TryUpdateModel(projectToUpdate, "", new string[] { "Name", "RepositoryProtocol", "URI", "User", "Password" }))
            {
                try
                {
                    _db.Entry(projectToUpdate).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(projectToUpdate);
        }

        // GET: /Projects/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(project);
        }

        // POST: /Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PostDelete(int id)
        {
            try
            {
                var project = _db.Projects.Find(id);
                _db.Projects.Remove(project);
                _db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        // GET: /Projects/Revisions/5
        public ActionResult Revisions(int? id)
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

        // GET: /Projects/Revision/5
        public ActionResult Revision(int id)
        {
            var revision = _db.Revisions.Find(id);
            if (revision == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(revision);
        }

        // GET: /Projects/Structure/5
        public JsonResult Structure(int id)
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