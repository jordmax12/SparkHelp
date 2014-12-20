using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparkHelp.ViewModels
{
    public class ResultsViewModel
    {
        public Question question { get; set; }
        public MSDN_table msdn { get; set; }
    }
}