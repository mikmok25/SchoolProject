using MySql.Data.MySqlClient;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;

namespace SchoolProject.Controllers
{
    public class ClassesDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        // <summary>
        // List the classes names in the system which match teacherid's, startdate, finishdate, classcode and classname.
        //</summary>
        //<returns>
        // A lists of classes
        //</returns>
        // <param name="SearchKey">The key to search teachers on</param>
        //<example>
        // GET api/ClassesData/ListClasses -> ["1", "Alexander", "Bennet", "August 05 2016", "55.3"]
        //</example>

        [HttpGet]
        [Route("api/ClassesData/ListClasses")]

        public IEnumerable<Classes> ListClasses()
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand Cmd = Conn.CreateCommand();

            string query = "SELECT * FROM classes";

            Cmd.CommandText = query;

            Cmd.Prepare();

            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            List<Classes> Classes = new List<Classes>();

            while (ResultSet.Read())
            {
                int classId = Convert.ToInt32(ResultSet["classid"]);
                string classCode = ResultSet["classcode"].ToString();
                int teacherId = Convert.ToInt32(ResultSet["teacherid"]);
                DateTime startDate = Convert.ToDateTime(ResultSet["startdate"]);
                string formattedStartDate = startDate.ToString("MMMM dd yyyy");
                DateTime finishDate = Convert.ToDateTime(ResultSet["finishdate"]);
                string formattedFinishDate = finishDate.ToString("MMMM dd yyyy");
                string className = ResultSet["classname"].ToString();

                Classes newClass = new Classes();

                newClass.classid = classId;
                newClass.classcode = classCode;
                newClass.teacherid = teacherId;
                newClass.startdate = formattedStartDate;
                newClass.finishdate = formattedFinishDate;
                newClass.classname = className;

                Classes.Add(newClass);
            }

            Conn.Close();
            return Classes;
        }
    }
}
