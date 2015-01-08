using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparkHelp.ViewModels
{
    public class ResultsViewModel
    {
        public StackOverflow stack { get; set; }
        public MSDN_table msdn { get; set; }
        public CodeProject CP { get; set; }
        public Unity3D unity { get; set; }
        public W3 w3 { get; set; }
        public string title { get; set; }
    }
}