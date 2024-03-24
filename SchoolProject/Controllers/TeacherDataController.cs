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
        // A lists of teacher data
        //</returns>
        // <param name="SearchKey">The key to search teachers on</param>
        //<example>
        // GET api/TeacherData/ListTeachers -> ["1", "Alexander", "Bennet", "August 05 2016", "55.3"]
        //</example>

        

        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey}")]

        public List<Teacher> ListTeachers( string SearchKey, string searchType)
        {

            // Create a connection to the school db
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server and database
            Conn.Open();

            // Establish a new command (query) for our db
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY
            string query = "";

            if(searchType == "empnum")
            {
                query = $"SELECT * FROM teachers WHERE employeenumber LIKE @searchkey";
            } else if (searchType == "hiredate")
            {
                query = $"SELECT * FROM teachers WHERE hiredate LIKE @searchkey";
            } else
            {
                query = $"SELECT * FROM teachers WHERE teacherfname LIKE @searchkey OR teacherlname LIKE @searchkey";
            }
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@searchkey", $"%{SearchKey}%");

            cmd.Prepare(); 

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
            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();

            // Open the database connection
            Conn.Open();

            // Create a MySqlCommand object
            MySqlCommand cmd = Conn.CreateCommand();

            // Define the SQL query
            string query = @"
        SELECT t.teacherfname, t.teacherlname, c.classid, c.classcode, c.classname, c.startdate, c.finishdate
        FROM teachers t
        LEFT JOIN classes c ON c.teacherid = t.teacherid
        WHERE t.teacherid = @TeacherId
    ";
            cmd.CommandText = query;

            // Add parameter for teacher ID
            cmd.Parameters.AddWithValue("@TeacherId", teacherId);

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create a Teacher object
            Teacher selectedTeacher = new Teacher();
            selectedTeacher.ClassesTaught = new List<Classes>();

            // Read data from the result set
            while (ResultSet.Read())
            {
                // Extract teacher information
                selectedTeacher.teacherFname = ResultSet["teacherfname"].ToString();
                selectedTeacher.teacherLname = ResultSet["teacherlname"].ToString();
                selectedTeacher.teacherId = teacherId;

                // Extract class information
                Classes cls = new Classes();
                cls.classcode = ResultSet["classcode"].ToString();
                cls.classname = ResultSet["classname"].ToString();
                cls.startdate = ResultSet["startdate"].ToString();
                cls.finishdate = ResultSet["finishdate"].ToString();

                // Add the class to the list of teacher's classes
                selectedTeacher.ClassesTaught.Add(cls);
            }

            // Close the database connection
            Conn.Close();

            // Return the populated Teacher object
            return selectedTeacher;
        }


    }
}
