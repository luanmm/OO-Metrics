using OOM.Core.Math;
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
    public class MetricsController : Controller
    {
        private OOMetricsContext _db = new OOMetricsContext();

        // GET: /Metrics
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /Metrics/List
        public ActionResult List()
        {
            return View(_db.Metrics.ToList());
        }

        // GET: /Metrics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var metric = _db.Metrics.Find(id);
            if (metric == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(metric);
        }

        // GET: /Metrics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Metrics/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Expression,TargetType")] Metric metric)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ExpressionEvaluator.Instance.Evaluate(metric.Expression);

                    _db.Metrics.Add(metric);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (ExpressionEvaluationException ex)
            {
                ModelState.AddModelError("", String.Format("The expression is invalid: {0}.", ex.Message));
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(metric);
        }

        // GET: /Metrics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var metric = _db.Metrics.Find(id);
            if (metric == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(metric);
        }

        // POST: /Metrics/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var metricToUpdate = _db.Metrics.Find(id);
            if (TryUpdateModel(metricToUpdate, "", new string[] { "Name", "Expression", "TargetType" }))
            {
                try
                {
                    _db.Entry(metricToUpdate).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(metricToUpdate);
        }

        // GET: /Metrics/Delete/5
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

            var metric = _db.Metrics.Find(id);
            if (metric == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(metric);
        }

        // POST: /Metrics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PostDelete(int id)
        {
            try
            {
                var metric = _db.Metrics.Find(id);
                _db.Metrics.Remove(metric);
                _db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
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