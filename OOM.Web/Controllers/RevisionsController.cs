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

        // GET: /Revisions/Explore/?elementType=class&elementId=8
        public JsonResult Explore(ElementType? elementType, int elementId)
        {
            ElementType type = elementType.HasValue ? elementType.Value : 0;
            int group = GetGroupByElementType(type);
            object result = null;
            switch (type)
            {
                case ElementType.Namespace:
                    var ns = _db.Namespaces.Find(elementId);
                    result = AssemblyExplorerStructure(new
                    {
                        id = ns.Id,
                        name = ns.Name,
                        description = ns.FullyQualifiedIdentifier,
                        group = group,
                        parent = new { group = GetGroupByElementType(null), id = ns.RevisionId }
                    }, ns.Classes);
                    break;
                case ElementType.Class:
                    var c = _db.Classes.Find(elementId);
                    result = AssemblyExplorerStructure(new
                    {
                        id = c.Id,
                        name = c.Name,
                        description = c.FullyQualifiedIdentifier,
                        group = group,
                        parent = new { group = GetGroupByElementType(ElementType.Namespace), id = c.NamespaceId }
                    }, c.Fields.Cast<IElement>().Concat(c.Methods));
                    break;
                case ElementType.Field:
                    var f = _db.Fields.Find(elementId);
                    result = AssemblyExplorerStructure(new
                    {
                        id = f.Id,
                        name = f.Name,
                        description = f.FullyQualifiedIdentifier,
                        group = group,
                        parent = new { group = GetGroupByElementType(ElementType.Class), id = f.ClassId }
                    }, null);
                    break;
                case ElementType.Method:
                    var m = _db.Methods.Find(elementId);
                    result = AssemblyExplorerStructure(new
                    {
                        id = m.Id,
                        name = m.Name,
                        description = m.FullyQualifiedIdentifier,
                        group = group,
                        parent = new { group = GetGroupByElementType(ElementType.Class), id = m.ClassId }
                    }, null);
                    break;
                default:
                    var r = _db.Revisions.Find(elementId);
                    result = AssemblyExplorerStructure(new
                    {
                        id = r.Id,
                        name = String.Format("Revision #{0}", r.Number),
                        description = String.Format("<p>{0}</p><p><strong>Author:</strong> {1}</p><p><strong>Date:</strong> {2:dd/mm/yyyy}</p>", r.Message, r.Author, r.CreatedAt),
                        group = group,
                        root = true
                    }, r.Namespaces);
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int GetGroupByElementType(ElementType? elementType)
        {
            ElementType type = elementType.HasValue ? elementType.Value : 0;
            int group = 0;
            switch (type)
            {
                case ElementType.Namespace:
                    group = 2;
                    break;
                case ElementType.Class:
                    group = 3;
                    break;
                case ElementType.Field:
                    group = 4;
                    break;
                case ElementType.Method:
                    group = 5;
                    break;
                default:
                    group = 1;
                    break;
            }

            return group;
        }

        private object AssemblyExplorerStructure(object rootData, IEnumerable<IElement> children)
        {
            var nodes = new List<object>();
            var links = new List<object>();

            var revisionNodeIndex = nodes.Count;
            nodes.Add(rootData);

            if (children != null)
            {
                foreach (var child in children)
                {
                    var nodeIndex = nodes.Count;

                    nodes.Add(new
                    {
                        id = child.Id,
                        name = child.FullyQualifiedIdentifier,
                        group = GetGroupByElementType(child.Type)
                    });

                    links.Add(new
                    {
                        source = revisionNodeIndex,
                        target = nodeIndex
                    });
                }
            }

            return new
            {
                nodes = nodes,
                links = links
            };
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