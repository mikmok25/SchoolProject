using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Lists()
        {
            // Should navigate to /Views/Teacher/Lists.cshtml
            List<Teacher> Teachers = new List<Teacher>();
            TeacherDataController controller = new TeacherDataController();
            //Teachers = controller.ListTeachers();
            return View(Teachers);
        }

        public ActionResult Show(int id)
        {
            // Should navigate to /Views/Teacher/Show.cshtml
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }
    }
}