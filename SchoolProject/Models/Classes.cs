using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.Models
{
    public class Classes
    {
        public int id { get; set; }
        public string classcode { get; set; }

        public string startdate { get; set; }

        public string finishdate { get; set; }

        public string classname { get; set; }
    }
}