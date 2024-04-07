using MySql.Data.MySqlClient;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolProject.Controllers
{
    public class StudentDataController : ApiController
    {

        private SchoolDbContext School = new SchoolDbContext();

        // <summary>
        // List the student id, first name, last name, number, enroll date.
        //</summary>
        //<returns>
        // A lists of students
        //</returns>
        // <param name="SearchKey">The key to search teachers on</param>
        //<example>
        // GET api/ClassesData/ListClasses -> ["1", "Alexander", "Bennet", "August 05 2016", "55.3"]
        //</example>

        [HttpGet]
        [Route("api/StudentData/ListStudents")]

        public IEnumerable<Student> ListStudents()
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand Cmd = Conn.CreateCommand();

            string query = "SELECT * FROM students";

            Cmd.CommandText = query;

            Cmd.Prepare();

            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            List<Student> Students = new List<Student>();

            while (ResultSet.Read())
            {
                int studentId = Convert.ToInt32(ResultSet["studentid"]);
                string studentFname = ResultSet["studentfname"].ToString();
                string studentLname = ResultSet["studentLname"].ToString();
                string studentNumber = ResultSet["studentnumber"].ToString() ;
                DateTime enrollDate = Convert.ToDateTime(ResultSet["enroldate"]);
                string formattedEnrollDate = enrollDate.ToString("MMMM dd yyyy");
         

                Student newStudent = new Student();

                newStudent.studentid = studentId;
                newStudent.studentfname = studentFname;
                newStudent.studentlname = studentLname;
                newStudent.studentnumber = studentNumber;
                newStudent.enroldate = formattedEnrollDate;

                Students.Add(newStudent);
            }

            Conn.Close();
            return Students;
        }
    }
}
