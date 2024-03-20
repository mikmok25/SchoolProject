using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.Models
{
    public class Teacher
    {
        //The following fields define a Teacher

        public int teacherId { get; set; }
        public string teacherFname { get; set;}

        public string teacherLname { get; set; }

        public string employeeNumber { get; set;}

        public double teacherSalary { get; set; }

        public string  hireDate { get; set; }




    }
}