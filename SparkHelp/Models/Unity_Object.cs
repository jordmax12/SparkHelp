using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparkHelp.Models
{
    public class Unity_Object
    {
        public string title { get; set; }
        public string link { get; set; }
        public string snippet { get; set; }
        public string query { get; set; }
        public string checkedAnswer { get; set; }
        public string[] uncheckedAnswers { get; set; }
    }
}