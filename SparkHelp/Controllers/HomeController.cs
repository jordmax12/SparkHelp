using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace SparkHelp.Controllers
{
    public class HomeController : Controller
    {
        SparkHelp_dbEntities db = new SparkHelp_dbEntities();

        public ActionResult Index(string queried)
        {
            Console.WriteLine(queried);
            if (Request.Params["q"] != null)
            {
                string query = Request.Params["q"].ToString();


                char[] delimChars = { ' ' };
                string[] newQuery = query.Split(delimChars);

                string resultQuery = "";
                int i = 0;
                foreach (string s in newQuery)
                {
                    if (s[i] == s[0])
                        resultQuery += s;
                    else
                        resultQuery += "+" + s;
                    i++;
                }

                GetLinks(resultQuery);
                return View();
            }

            return View();
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
                        question.QuestionLink = rows.Attributes["href"].Value;
                        question.QuestionTitle = rows.InnerHtml.Trim();
                        string sub_url = "http://stackoverflow.com" + rows.Attributes["href"].Value;
                        //Console.WriteLine(rows.Attributes["href"].Value);

                        HtmlWeb sub_web = new HtmlWeb();
                        HtmlDocument sub_doc = sub_web.Load(sub_url);
                        if (sub_doc.DocumentNode.SelectNodes("//div [@class=\"answer accepted-answer\"]") != null)
                        {
                            foreach (HtmlNode sub_node in sub_doc.DocumentNode.SelectNodes("//div [@class=\"answer accepted-answer\"]"))
                            {
                                if (sub_node == null)
                                    Console.WriteLine("debug");
                                else
                                {
                                    Console.WriteLine(rows.InnerHtml);
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
                                                    
                                                    Console.WriteLine(sub_node3.InnerHtml);
                                                }

                                            }
                                        }
                                    }
                                }

                                //sub_rows.SelectNodes(".//p");

                            }
                        }
                        //db.Questions.Add(question);
                        //db.SaveChanges();
                        string connString = System.Configuration.ConfigurationManager.ConnectionStrings[@"SparkHelp"].ConnectionString;
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            conn.Open();

                            if (conn.State != System.Data.ConnectionState.Open)
                            {
                                Console.WriteLine("Error in opening database connection!");
                                return;
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT INTO Questions (QuestionTitle, QuestionLink) VALUES (@title, @link)", conn))
                            {
                                SqlParameter qTitle = new SqlParameter();
                                qTitle.ParameterName = "@title";
                                qTitle.Value = question.QuestionTitle;

                                SqlParameter qLink = new SqlParameter();
                                qLink.ParameterName = "@link";
                                qLink.Value = question.QuestionLink;

                                cmd.Parameters.Add(qTitle);
                                cmd.Parameters.Add(qLink);

                                cmd.ExecuteNonQuery();
                                conn.Close();
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