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
                    object element = null;
                    switch (metric.TargetType) 
                    {
                        case ElementType.Namespace:
                            element = new Namespace();
                            break;
                        case ElementType.Class:
                            element = new Class();
                            break;
                        case ElementType.Field:
                            element = new Field();
                            break;
                        case ElementType.Method:
                            element = new Method();
                            break;
                    }
                    EvaluateMetric(metric, element);

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

        // POST: /Metrics/History/?revisionId=5&elementType=Class&elementId=8
        public JsonResult History(int revisionId, string elementType, int elementId)
        {
            var revision = _db.Revisions.Find(revisionId);
            if (revision == null)
                throw new HttpException(400, "Bad request.");

            var metricType = ElementType.Namespace;
            object element = null;
            if (elementType.Equals("Namespace", StringComparison.InvariantCultureIgnoreCase))
            {
                metricType = ElementType.Namespace;
                element = _db.Namespaces.Find(elementId);
            }
            else if (elementType.Equals("Class", StringComparison.InvariantCultureIgnoreCase))
            {
                metricType = ElementType.Class;
                element = _db.Classes.Find(elementId);
            }
            else if (elementType.Equals("Field", StringComparison.InvariantCultureIgnoreCase))
            {
                metricType = ElementType.Field;
                element = _db.Fields.Find(elementId);
            }
            else if (elementType.Equals("Method", StringComparison.InvariantCultureIgnoreCase))
            {
                metricType = ElementType.Method;
                element = _db.Methods.Find(elementId);
            }

            if (element == null)
                throw new HttpException(400, "Bad request.");

            var data = new List<object>();
            var metrics = _db.Metrics.Where(x => x.TargetType == metricType);
            var relatedRevisions = FindRelatedRevisions(revision, element);

            foreach (var metric in metrics)
            {
                var dates = new List<object>();
                foreach (var relatedRevision in relatedRevisions)
                {
                    var result = Convert.ToInt32(EvaluateMetric(metric, relatedRevision.Value)); // TODO: Remove this kind of code (just needed to example how this chart type works)
                    while (result-- > 0)
                        dates.Add(relatedRevision.Key.CreatedAt);
                }

                data.Add(new
                { 
                    name = metric.Name,
                    dates = dates
                }); 
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Privates

        private IDictionary<Revision, object> FindRelatedRevisions(Revision baseRevision, object element)
        {
            if (element is Namespace)
                return _db.Namespaces
                    .Where(x => x.Revision.ProjectId == baseRevision.ProjectId && x.FullyQualifiedIdentifier.Equals((element as Namespace).FullyQualifiedIdentifier))
                    .ToDictionary(x => x.Revision, n => n as object);
            
            if (element is Class)
                return _db.Classes
                    .Where(x => x.Namespace.Revision.ProjectId == baseRevision.ProjectId && x.FullyQualifiedIdentifier.Equals((element as Class).FullyQualifiedIdentifier))
                    .ToDictionary(x => x.Namespace.Revision, x => x as object);

            if (element is Field)
                return _db.Fields
                    .Where(x => x.Class.Namespace.Revision.ProjectId == baseRevision.ProjectId && x.FullyQualifiedIdentifier.Equals((element as Field).FullyQualifiedIdentifier))
                    .ToDictionary(x => x.Class.Namespace.Revision, x => x as object);

            if (element is Method)
                return _db.Methods
                    .Where(x => x.Class.Namespace.Revision.ProjectId == baseRevision.ProjectId && x.FullyQualifiedIdentifier.Equals((element as Method).FullyQualifiedIdentifier))
                    .ToDictionary(x => x.Class.Namespace.Revision, x => x as object);

            throw new ArgumentException("This element is not from an expected type.");
        }

        private decimal EvaluateMetric(Metric metric, object element = null)
        {
            if (element == null)
                return ExpressionEvaluator.Instance.Evaluate(metric.Expression);

            return ExpressionEvaluator.Instance.Evaluate(metric.Expression, GetMetricParameters(element));
        }

        private IDictionary<string, object> GetMetricParameters(object element)
        {
            var parameters = new Dictionary<string, object>();

            if (element is Namespace)
            {
                var n = element as Namespace;
                // TODO
            }
            else if (element is Class)
            {
                var c = element as Class;
                // TODO
            }
            else if (element is Field)
            {
                var f = element as Field;
                // TODO
            }
            else if (element is Method) 
            {
                var m = element as Method;
                parameters.Add("loc", m.LineCount);
                parameters.Add("ep", m.ExitPoints);
            }

            return parameters;
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