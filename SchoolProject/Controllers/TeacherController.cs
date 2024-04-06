using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace SchoolProject.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Lists(string SearchKey, string searchType)
        {
            Debug.WriteLine($"The search key is:{SearchKey}");
            // Should navigate to /Views/Teacher/Lists.cshtml
            List<Teacher> Teachers = new List<Teacher>();
            TeacherDataController controller = new TeacherDataController();
            Teachers = controller.ListTeachers(SearchKey, searchType);
            return View(Teachers);
        }

        public ActionResult Show(int id)
        {
            // Should navigate to /Views/Teacher/Show.cshtml
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        // GET: Teacher/New -> a webpage asking the user to insert new teacher information.

        public ActionResult New()
        {
            //navigate to /Views/Teacher/New.cshtml
            return View();
        }

        // POST: Teacher / Create -> Redirects to the teachers lists page.

        [HttpPost]
        public ActionResult Create(string teacherfname, string teacherlname, string employeeNumber, string hiredate, decimal? salary)
        {

            Teacher NewTeacher = new Teacher();
            NewTeacher.teacherFname = teacherfname;
            NewTeacher.teacherLname = teacherlname;
            NewTeacher.employeeNumber = employeeNumber;
            NewTeacher.hireDate = hiredate;
            NewTeacher.teacherSalary = salary;

            TeacherDataController DataController = new TeacherDataController();
            DataController.AddTeacher(NewTeacher);





            return RedirectToAction("Lists");
        }

        // GET: Teacher/DeleteConfirm/{id} -> a webpage that ask if the user really wants to delete the specific teacher.

        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);
            return RedirectToAction("Lists");
        }








    }


}