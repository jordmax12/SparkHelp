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
    
    public partial class Question
    {
        public Question()
        {
            this.CheckedAnswers = new HashSet<CheckedAnswer>();
            this.UncheckedAnswers = new HashSet<UncheckedAnswer>();
            this.MSDN_table = new HashSet<MSDN_table>();
        }
    
        public int QuestionID { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionLink { get; set; }
        public string QuestionText { get; set; }
        public Nullable<int> QuestionVote { get; set; }
        public string QuestionQuery { get; set; }
    
        public virtual ICollection<CheckedAnswer> CheckedAnswers { get; set; }
        public virtual ICollection<UncheckedAnswer> UncheckedAnswers { get; set; }
        public virtual ICollection<MSDN_table> MSDN_table { get; set; }
    }
}
