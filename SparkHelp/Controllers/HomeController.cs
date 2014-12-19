using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using PagedList;

namespace SparkHelp.Controllers
{
    public class HomeController : Controller
    {
        SparkHelp_dbEntities db = new SparkHelp_dbEntities();
        string resultQuery = "";
        public ActionResult Index(string queried, int? page)
        {
            Console.WriteLine(queried);
            if (Request.Params["q"] != null)
            {
                string query = Request.Params["q"];


                char[] delimChars = { ' ' };
                string[] newQuery = query.Split(delimChars);

                
                int i = 0;
                foreach (string s in newQuery)
                {
                    if (s[i] == s[0])
                        resultQuery += s;
                    else
                        resultQuery += "+" + s;
                    i++;
                }


                var grabQuestions = db.Questions.Where(q => q.QuestionQuery == resultQuery);
                var count = grabQuestions.ToList().Count;
                if(count == 0)
                    GetLinks(resultQuery);
                
                List<Question> list = grabQuestions.ToList();
                return View(grabQuestions.ToList().ToPagedList(page ?? 1, 15));
            }
            List<Question> emptyList = new List<Question>();
            return View(emptyList.ToPagedList(page ?? 1, 6));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void GetLinks(string query)
        {
            string url = "http://stackoverflow.com/search?q=" + query;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            List<string> results = new List<string>();
            

            if (doc.DocumentNode.SelectNodes("//div [@class=\"result-link\"]") != null)
            {
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div [@class=\"result-link\"]"))
                {

                    if (node == null)
                        Console.WriteLine("debug");
                    else
                    {
                        Question question = new Question();
                        HtmlNode rows = node.SelectSingleNode(".//a");

                        string sub_url = "http://stackoverflow.com" + rows.Attributes["href"].Value;
                        //Console.WriteLine(rows.Attributes["href"].Value);
                        question.QuestionLink = rows.Attributes["href"].Value;
                        question.QuestionTitle = rows.InnerHtml.Trim();
                        question.QuestionQuery = resultQuery;
                        HtmlWeb sub_web = new HtmlWeb();
                        HtmlDocument sub_doc = sub_web.Load(sub_url);
                        if (rows.InnerHtml.Trim().StartsWith("Q"))
                        {
                            if (sub_doc.DocumentNode.SelectNodes("//div [@class=\"answer accepted-answer\"]") != null)
                            {

                                foreach (HtmlNode sub_node in sub_doc.DocumentNode.SelectNodes("//div [@class=\"answer accepted-answer\"]"))
                                {
                                    if (sub_node == null)
                                        Console.WriteLine("debug");
                                    else
                                    {
                                        
                                        foreach (HtmlNode sub_node2 in sub_node.SelectNodes(".//div [@class=\"post-text\"]"))
                                        {
                                            if (sub_node2 == null)
                                                Console.WriteLine("debug");
                                            else
                                            {
                                                foreach (HtmlNode sub_node3 in sub_node2.SelectNodes(".//p"))
                                                {
                                                    if (sub_node3 == null)
                                                        Console.WriteLine("debug");
                                                    else
                                                    {

                                                        
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }



                        if (sub_doc.DocumentNode.SelectNodes("//div [@class=\"question\"]") != null)
                        {
                            foreach (HtmlNode sub_node in sub_doc.DocumentNode.SelectNodes(".//div [@class=\"question\"]"))
                            {
                                foreach (HtmlNode sub_node2 in sub_node.SelectNodes(".//div [@class=\"post-text\"]"))
                                {
                                    foreach (HtmlNode sub_node3 in sub_node2.SelectNodes(".//p"))
                                    {
                                        if (question.QuestionText == null)
                                            question.QuestionText = sub_node3.InnerHtml;
                                        else
                                            question.QuestionText += " " + sub_node3.InnerHtml;
                                    }
                                }


                            }
                            Console.WriteLine(  "s");
                        }

                        if (rows.InnerHtml.Trim().StartsWith("Q"))
                        {
                            string connString = System.Configuration.ConfigurationManager.ConnectionStrings[@"SparkHelp"].ConnectionString;
                            using (SqlConnection conn = new SqlConnection(connString))
                            {
                                conn.Open();

                                if (conn.State != System.Data.ConnectionState.Open)
                                {
                                    Console.WriteLine("Error in opening database connection!");
                                    return;
                                }

                                using (SqlCommand cmd = new SqlCommand("INSERT INTO Questions (QuestionTitle, QuestionLink, QuestionText, QuestionQuery) VALUES (@title, @link, @text, @query)", conn))
                                {
                                    SqlParameter qTitle = new SqlParameter();
                                    qTitle.ParameterName = "@title";
                                    string strip = question.QuestionTitle;
                                    strip = strip.Substring(3);
                                    question.QuestionTitle = strip;
                                    qTitle.Value = question.QuestionTitle;

                                    SqlParameter qLink = new SqlParameter();
                                    qLink.ParameterName = "@link";
                                    qLink.Value = question.QuestionLink;

                                    SqlParameter qText = new SqlParameter();
                                    qText.ParameterName = "@text";
                                    string trunc = question.QuestionText;

                                    if (trunc.Length > 1000)
                                    {
                                        trunc = trunc.Substring(0, 1000);
                                    }

                                    question.QuestionText = trunc;
                                    qText.Value = question.QuestionText;

                                    SqlParameter qQuery = new SqlParameter();
                                    qQuery.ParameterName = "@query";
                                    qQuery.Value = question.QuestionQuery;

                                    cmd.Parameters.Add(qTitle);
                                    cmd.Parameters.Add(qLink);
                                    cmd.Parameters.Add(qText);
                                    cmd.Parameters.Add(qQuery);
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }
                        }
                       
                    }
                }
            }
            else
            {
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div [@class=\"question-summary\"]"))
                {
                    Console.WriteLine(node.InnerHtml);
                }
            }
        }
    }


}
