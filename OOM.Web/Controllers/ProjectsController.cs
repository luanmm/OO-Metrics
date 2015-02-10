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

        // GET: /Projects/
        public ActionResult Index()
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

            Project project = _db.Projects.Find(id);
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

            Project project = _db.Projects.Find(id);
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

            Project project = _db.Projects.Find(id);
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
                Project project = _db.Projects.Find(id);
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

            Project project = _db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(project.Revisions);
        }

        // GET: /Projects/Revision/5
        public ActionResult Revision(int id)
        {
            Revision revision = _db.Revisions.Find(id);
            if (revision == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(revision);
        }

        // GET: /Projects/Structure/5
        public JsonResult Structure(int id)
        {
            return Json(new
            {
                nodes = new[] {
                new {
                id = "n0",
                label = "A node",
                x = 0,
                y = 0,
                size = 3
                },
                new {
                id = "n1",
                label = "Another node",
                x = 3,
                y = 1,
                size = 2
                },
                new {
                id = "n2",
                label = "And a last one",
                x = 1,
                y = 3,
                size = 1
                }
                },
                edges = new[] {
                new {
                  id = "e0",
                  source = "n0",
                  target = "n1"
                },
                new {
                  id = "e1",
                  source = "n1",
                  target = "n2"
                },
                new {
                  id = "e2",
                  source = "n2",
                  target = "n0"
                }
              }
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