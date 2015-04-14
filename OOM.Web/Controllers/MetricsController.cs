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

        #region Actions

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
                    EvaluateMetric(metric, (IElement)Activator.CreateInstance(metric.TargetType.ToElement()));

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
                    EvaluateMetric(metricToUpdate, (IElement)Activator.CreateInstance(metricToUpdate.TargetType.ToElement()));

                    _db.Entry(metricToUpdate).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (ExpressionEvaluationException ex)
                {
                    ModelState.AddModelError("", String.Format("The expression is invalid: {0}.", ex.Message));
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

        // GET: /Metrics/ByElementType/?elementType=class
        public JsonResult ByElementType(ElementType elementType)
        {
            var metrics = _db.Metrics.Where(x => x.TargetType == elementType)
                .OrderBy(x => x.Name)
                .Select(x => new { value = x.Id, name = x.Name });

            return Json(metrics, JsonRequestBehavior.AllowGet);
        }

        // GET: /Metrics/History/?elementType=class&elementId=8&revisionId=5
        public JsonResult History(ElementType elementType, int elementId, int revisionId, int metricId)
        {
            var revision = _db.Revisions.Find(revisionId);
            if (revision == null)
                throw new HttpException(400, "Bad request.");

            var element = _db.Set(elementType.ToElement()).Find(elementId) as IElement;
            if (element == null)
                throw new HttpException(400, "Bad request.");

            var data = new List<object>();
            var metric = _db.Metrics.Find(metricId);
            var relatedRevisions = FindRelatedRevisions(revision, element);
            foreach (var relatedRevision in relatedRevisions)
            {
                var result = EvaluateMetric(metric, relatedRevision.Item2);
                data.Add(new
                {
                    revision = new {
                        number = relatedRevision.Item1.Id, // TODO: Number of revision (incremental)
                        rid = relatedRevision.Item1.RID,
                        author = relatedRevision.Item1.Author,
                        message = relatedRevision.Item1.Message,
                        createdAt = relatedRevision.Item1.CreatedAt
                    },
                    value = result
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Privates

        private IEnumerable<Tuple<Revision, IElement>> FindRelatedRevisions(Revision baseRevision, IElement element)
        {
            if (element.Type == ElementType.Namespace)
                return _db.Namespaces
                    .Where(x => x.Revision.ProjectId == baseRevision.ProjectId && element.FullyQualifiedIdentifier.Equals(x.FullyQualifiedIdentifier)).ToList()
                    .Select(x => Tuple.Create<Revision, IElement>(x.Revision, x)).ToList();

            if (element.Type == ElementType.Class)
                return _db.Classes
                    .Where(x => x.Namespace.Revision.ProjectId == baseRevision.ProjectId && element.FullyQualifiedIdentifier.Equals(x.FullyQualifiedIdentifier)).ToList()
                    .Select(x => Tuple.Create<Revision, IElement>(x.Namespace.Revision, x)).ToList();

            if (element.Type == ElementType.Field)
                return _db.Fields
                    .Where(x => x.Class.Namespace.Revision.ProjectId == baseRevision.ProjectId && element.FullyQualifiedIdentifier.Equals(x.FullyQualifiedIdentifier)).ToList()
                    .Select(x => Tuple.Create<Revision, IElement>(x.Class.Namespace.Revision, x)).ToList();

            if (element.Type == ElementType.Method)
                return _db.Methods
                    .Where(x => x.Class.Namespace.Revision.ProjectId == baseRevision.ProjectId && element.FullyQualifiedIdentifier.Equals(x.FullyQualifiedIdentifier)).ToList()
                    .Select(x => Tuple.Create<Revision, IElement>(x.Class.Namespace.Revision, x)).ToList();

            throw new ArgumentException("This element is not from an expected type.");
        }

        private decimal EvaluateMetric(Metric metric, IElement element = null)
        {
            if (element == null)
                return ExpressionEvaluator.Instance.Evaluate(metric.Expression);

            return ExpressionEvaluator.Instance.Evaluate(metric.Expression, element.Parameters);
        }

        #endregion

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