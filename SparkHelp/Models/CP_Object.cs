using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparkHelp.Models
{
    public class CP_Object
    {
        public string title { get; set; }
        public string link { get; set; }
        public float rating { get; set; }
        public int votes { get; set; }
        public string summary { get; set; }
        public string query { get; set; }
    }
}