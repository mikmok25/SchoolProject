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
                int teacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string teacherFname = ResultSet["teacherfname"].ToString();
                string teacherLname = ResultSet["teacherlname"].ToString();
                string employeeNumber = ResultSet["employeenumber"].ToString();
                DateTime hireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                object salaryObj = ResultSet["salary"];
                decimal teacherSalary;

                if (salaryObj != DBNull.Value)
                {
                    teacherSalary = Convert.ToDecimal(salaryObj);
                }
                else
                {
                    // Handle the case where salary is DBNull, perhaps by assigning a default value
                    teacherSalary = Convert.ToDecimal(0.00); // Or any other appropriate default value
                }



                Teacher newTeacher = new Teacher();

                newTeacher.teacherFname = teacherFname;
                newTeacher.teacherLname = teacherLname;
                newTeacher.teacherId = teacherId;
                newTeacher.employeeNumber = employeeNumber;
                newTeacher.hireDate = hireDate;
                newTeacher.teacherSalary = teacherSalary;

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
SELECT t.teacherid, t.teacherfname, t.teacherlname, t.employeenumber, t.hiredate, t.salary, c.classid, c.classcode, c.classname, c.startdate, c.finishdate
FROM teachers t
LEFT JOIN classes c ON c.teacherid = t.teacherid
WHERE t.teacherid = @TeacherId";

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
                selectedTeacher.teacherId = teacherId;
                selectedTeacher.teacherFname = ResultSet["teacherfname"].ToString();
                selectedTeacher.teacherLname = ResultSet["teacherlname"].ToString();
                selectedTeacher.employeeNumber = ResultSet["employeenumber"].ToString();
                selectedTeacher.hireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                selectedTeacher.teacherSalary = Convert.ToDecimal(ResultSet["salary"]);




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

        /// <summary>
        /// Receives teacher information and adds it to the database
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        /// <example>
        /// POST localhost:xx/api/teacherdata/addteacher
        /// FORM DATA / POST DATA / REQUEST BODY
        /// {
        ///     "teacherfname": "Alexander",
        ///     "teacherlname": "Bennett"
        /// }
        /// </example>
        // Add teacher
        [HttpPost]

        public void AddTeacher([FromBody]Teacher NewTeacher)
        {

            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            string query = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@fname, @lname, @empnum, @hiredate, @salary)";

            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@fname", NewTeacher.teacherFname);
            cmd.Parameters.AddWithValue("@lname", NewTeacher.teacherLname);
            cmd.Parameters.AddWithValue("@empnum", NewTeacher.employeeNumber);
            cmd.Parameters.AddWithValue("@hiredate", NewTeacher.hireDate);
            cmd.Parameters.AddWithValue("@salary", NewTeacher.teacherSalary);

            Debug.WriteLine(cmd.CommandText);

            cmd.Prepare();
            cmd.ExecuteNonQuery(); // DML operations

            Conn.Close();


        }

        // Delete Teacher By its ID
        /// <summary>
        /// Deletes a Teacher from the connected MySQL Database if the ID of that teacher exists. Does NOT maintain relational integrity.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <example>POST /api/TeacherData/DeleteTeacher/3</example>
        [HttpDelete]
        [Route("api/TeacherData/DeleteTeacher/{teacherId}")]

        public void DeleteTeacher(int id)
        {
            // Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server and database
            Conn.Open();

            // Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY
            cmd.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
            Conn.Close();
        }

        /// <summary>
        ///  Recieves a teacher id and updated teacher information and 
        ///  update the corresponding teacher in the database.
        /// </summary>
        /// <example>
        /// curl -d @teacher.json -H "Content-Type: application/json" http://localhost:53661/api/TeacherData/UpdateTeacher/10
        /// POST api/TeacherData/UpdateTeacher/{teacherId}
        /// {
        ///     "teacherfname": "John",
        ///     "teacherlname": "Doe",
        ///     "employeenumber": "T445"
        ///     "hiredate": "2021-04-15",
        ///     "salary": 32.14
        /// }
        /// </example>
        /// <returns>
        /// 
        /// </returns>

        [HttpPost]
        [Route("api/TeacherData/UpdateTeacher/{TeacherId}")]

        public void UpdateTeacher(int TeacherId, [FromBody] Teacher UpdatedTeacher)
        {
            //Debug.WriteLine("Teacher First Name: " + UpdatedTeacher.teacherFname);
            //Debug.WriteLine("Teacher Last Name: " + UpdatedTeacher.teacherLname);
            //Debug.WriteLine("Teacher Employee Number: " + UpdatedTeacher.employeeNumber);
            //Debug.WriteLine("Teacher Hiredate: " + UpdatedTeacher.hireDate);
            //Debug.WriteLine("Teacher Salary: " + Convert.ToDecimal(UpdatedTeacher.teacherSalary));

            string query = "UPDATE teachers SET teacherfname=@teacherfname, teacherlname=@teacherlname, employeenumber=@employeenumber, hiredate=@hiredate, salary=@salary WHERE teacherid=@teacherid";

            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();


            MySqlCommand Cmd = Conn.CreateCommand();
            
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherfname", UpdatedTeacher.teacherFname);
            Cmd.Parameters.AddWithValue("@teacherlname", UpdatedTeacher.teacherLname);
            Cmd.Parameters.AddWithValue("@employeenumber", UpdatedTeacher.employeeNumber);
            Cmd.Parameters.AddWithValue("@hiredate", UpdatedTeacher.hireDate);
            Cmd.Parameters.AddWithValue("@salary", UpdatedTeacher.teacherSalary);
            Cmd.Parameters.AddWithValue("@teacherid", TeacherId);

            Cmd.Prepare();

            Cmd.ExecuteNonQuery();
            Conn.Close();

            return;






        }

    }
}
