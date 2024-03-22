using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SchoolProject.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Globalization;
using System.Diagnostics;

namespace SchoolProject.Controllers
{
    public class TeacherDataController : ApiController
    {
        

        private SchoolDbContext School = new SchoolDbContext();
        // <summary>
        // List the teacher names in the system which match teacher names, hiredate or salary.
        //</summary>
        //<returns>
        // A lists of teachers
        //</returns>
        //<example>
        // GET api/TeacherData/ListTeachers -> ["Alexander Bennet", "Jane Doe", "Steve Smith", ...]
        //</example>

        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey}")]

        public List<Teacher> ListTeachers(string SearchKey)
        {
            Debug.WriteLine("The search key is:" + SearchKey);
            // Create a connection to the school db
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server and database
            Conn.Open();

            // Establish a new command (query) for our db
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY

            cmd.CommandText = "SELECT * FROM teachers";

            // Gather Result from Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create an empty list of Teacher names
            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                // Access column information by the DB column name as index

                // string TeacherName = ResultSet["teacherfname"] + " " + ResultSet["teacherlname"];

                string teacherFname = ResultSet["teacherfname"].ToString();
                string teacherLname = ResultSet["teacherlname"].ToString();
                string employeeNumber = ResultSet["employeenumber"].ToString();
                double teacherSalary = Convert.ToDouble(ResultSet["salary"]);
                DateTime hireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                string formattedDate = hireDate.ToString("MMMM dd yyyy");


                int teacherId = Convert.ToInt32(ResultSet["teacherid"]);

                Teacher newTeacher = new Teacher();

                newTeacher.teacherFname = teacherFname;
                newTeacher.teacherLname = teacherLname;
                newTeacher.teacherId = teacherId;
                newTeacher.employeeNumber = employeeNumber;
                newTeacher.teacherSalary = teacherSalary;
                newTeacher.hireDate = formattedDate;



                Teachers.Add(newTeacher);

            }

            // Close the connection between the MySQL Database and the Webserver
            Conn.Close();

            return Teachers;
        }

        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{teacherId}")]

        public Teacher FindTeacher(int teacherId)
        {
            // Create a connection to database
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection to DB
            Conn.Open();

            // Create a mysql command
            MySqlCommand cmd = Conn.CreateCommand();

            // Using a query to access information

            string query = "SELECT * FROM teachers WHERE teacherid = " + teacherId;
            cmd.CommandText = query;

            // Run the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create Teacher Obj
            Teacher SelectedTeacher = new Teacher();

            // Put that information into Teacher obj.


            while (ResultSet.Read())
            {
                string teacherFname = ResultSet["teacherfname"].ToString();
                string teacherLname = ResultSet["teacherlname"].ToString();
                string employeeNumber = ResultSet["employeenumber"].ToString();
                double teacherSalary = Convert.ToDouble(ResultSet["salary"]);
                DateTime hireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                string formattedDate = hireDate.ToString("MMMM dd yyyy");

                SelectedTeacher.teacherFname = teacherFname;
                SelectedTeacher.teacherLname = teacherLname;
                SelectedTeacher.teacherId = teacherId;
                SelectedTeacher.employeeNumber = employeeNumber;
                SelectedTeacher.teacherSalary = teacherSalary;
                SelectedTeacher.hireDate = formattedDate;


            }

            return SelectedTeacher;

        }
    }
}
