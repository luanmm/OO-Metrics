using OOM.Model;
using OOM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OOM.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private static readonly ProjectRepository _repository = new ProjectRepository();

        // GET: /Projects/
        public ActionResult Index()
        {
            return View(_repository.GetAll());
        }

        // GET: /Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = _repository.GetById(id.Value);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,RepositoryProtocol,URI,User,Password")] Project project)
        {
            if (ModelState.IsValid)
            {
                _repository.Create(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: /Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = _repository.GetById(id.Value);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(project);
        }

        // POST: /Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,RepositoryProtocol,URI,User,Password")] Project project)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: /Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = _repository.GetById(id.Value);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(project);
        }

        // POST: /Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}