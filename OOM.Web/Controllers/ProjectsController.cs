using OOM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OOM.Web.Controllers
{
    public class ProjectsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var repository = new ProjectRepository();
            ViewBag.Projects = repository.GetAll();
            return View();
        }
    }
}