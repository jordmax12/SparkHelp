//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SparkHelp
{
    using System;
    using System.Collections.Generic;
    
    public partial class MSDN_table
    {
        public int QueryID { get; set; }
        public string QuerySearch { get; set; }
        public Nullable<int> QuestionID { get; set; }
        public string QueryTitle { get; set; }
        public string QueryDescription { get; set; }
        public string QueryURL { get; set; }
    
        public virtual Question Question { get; set; }
    }
}