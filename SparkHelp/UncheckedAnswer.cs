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
    
    public partial class UncheckedAnswer
    {
        public int AnswerID { get; set; }
        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public Nullable<int> AnswerVote { get; set; }
    
        public virtual Question Question { get; set; }
    }
}
