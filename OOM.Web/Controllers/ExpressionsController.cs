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
    public class ExpressionsController : Controller
    {
        private OOMetricsContext _db = new OOMetricsContext();

        #region Actions

        // GET: /Expressions
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /Expressions/List
        public ActionResult List()
        {
            return View(_db.Expressions.ToList());
        }

        // GET: /Expressions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expression = _db.Expressions.Find(id);
            if (expression == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(expression);
        }

        // GET: /Expressions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Expressions/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Formula,TargetType")] Expression expression)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sampleElement = (IElement)Activator.CreateInstance(expression.TargetType.ToElement());
                    if (sampleElement == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ExpressionEvaluator.Instance.Evaluate(expression.Formula, sampleElement.Parameters);

                    _db.Expressions.Add(expression);
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

            return View(expression);
        }

        // GET: /Expressions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expression = _db.Expressions.Find(id);
            if (expression == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(expression);
        }

        // POST: /Expressions/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expressionToUpdate = _db.Expressions.Find(id);
            if (TryUpdateModel(expressionToUpdate, "", new string[] { "Name", "Formula", "TargetType" }))
            {
                try
                {
                    var sampleElement = (IElement)Activator.CreateInstance(expressionToUpdate.TargetType.ToElement());
                    if (sampleElement == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ExpressionEvaluator.Instance.Evaluate(expressionToUpdate.Formula, sampleElement.Parameters);

                    _db.ExpressionsResult.RemoveRange(_db.ExpressionsResult.Where(x => x.ExpressionId == expressionToUpdate.Id));

                    _db.Entry(expressionToUpdate).State = EntityState.Modified;
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

            return View(expressionToUpdate);
        }

        // GET: /Expressions/Delete/5
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

            var expression = _db.Expressions.Find(id);
            if (expression == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(expression);
        }

        // POST: /Expressions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PostDelete(int id)
        {
            try
            {
                var expression = _db.Expressions.Find(id);
                if (expression == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                _db.ExpressionsResult.RemoveRange(_db.ExpressionsResult.Where(x => x.ExpressionId == expression.Id));

                _db.Expressions.Remove(expression);
                _db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        // GET: /Expressions/ByElementType/?elementType=class
        public JsonResult ByElementType(ElementType elementType)
        {
            var expressions = _db.Expressions.Where(x => x.TargetType == elementType)
                .OrderBy(x => x.Name)
                .Select(x => new { value = x.Id, name = x.Name });

            return Json(expressions, JsonRequestBehavior.AllowGet);
        }

        // GET: /Expressions/History/?elementType=class&elementId=8&revisionId=5
        public JsonResult History(ElementType elementType, int elementId, int revisionId, int expressionId)
        {
            var revision = _db.Revisions.Find(revisionId);
            if (revision == null)
                throw new HttpException(400, "Bad request.");

            var element = _db.Set(elementType.ToElement()).Find(elementId) as IElement;
            if (element == null)
                throw new HttpException(400, "Bad request.");

            var data = new List<object>();
            var expression = _db.Expressions.Find(expressionId);
            var relatedRevisions = FindRelatedRevisions(revision, element);

            decimal lastResult = Decimal.MinValue;
            foreach (var relatedRevision in relatedRevisions)
            {
                var result = EvaluateExpression(expression, relatedRevision.Item2);
                if (result == lastResult)
                    continue;

                data.Add(new
                {
                    revision = new {
                        number = relatedRevision.Item1.Number,
                        rid = relatedRevision.Item1.RID,
                        author = relatedRevision.Item1.Author,
                        message = relatedRevision.Item1.Message,
                        createdAt = relatedRevision.Item1.CreatedAt
                    },
                    value = result
                });

                lastResult = result;
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

        private decimal EvaluateExpression(Expression expression, IElement element)
        {
            var cachedResult = _db.ExpressionsResult.FirstOrDefault(x => x.ElementId == element.Id && x.ElementType.HasFlag(element.Type) && x.ExpressionId == expression.Id);
            if (cachedResult != null)
                return cachedResult.Result;

            var result = ExpressionEvaluator.Instance.Evaluate(expression.Formula, element.Parameters);
            _db.ExpressionsResult.Add(new ExpressionResult
            {
                ElementId = element.Id,
                ElementType = element.Type,
                ExpressionId = expression.Id,
                Result = result
            });
            _db.SaveChanges();

            return result;
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