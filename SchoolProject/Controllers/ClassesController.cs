using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class ClassesController : Controller
    {
        // GET: Classes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            ClassesDataController controller = new ClassesDataController();
            IEnumerable<Classes> Classes = controller.ListClasses();
            return View(Classes);
        }
    }
}