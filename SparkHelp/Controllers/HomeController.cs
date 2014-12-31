﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using HtmlAgilityPack;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using SparkHelp.Models;
using SparkHelp.ViewModels;
using StackExchange.StacMan;
using StackExchange.StacMan.Questions;

namespace SparkHelp.Controllers
{
    public class HomeController : Controller
    {
        SparkHelp_dbEntities db = new SparkHelp_dbEntities();
        string resultQuery = "";
        string msdn_resultQuery = "";
        public ActionResult Index(string queried, int? page)
        {
            Console.WriteLine(queried);
            if (Request.Params["q"] != null)
            {
                string query = Request.Params["q"];


                char[] delimChars = { ' ' };
                string[] newQuery = query.Split(delimChars);

                
                int i = 0;
                int j = 0;
                foreach (string s in newQuery)
                {
                    if (s[i] == s[0])
                        resultQuery += s;
                    else
                        resultQuery += "+" + s;
                    i++;
                }

                foreach (string st in newQuery)
                {
                    if (st[j] == st[0])
                        msdn_resultQuery += st;
                    else
                        msdn_resultQuery += "%20" + st;
                    j++;
                }

                


                var grabQuestions = db.Questions.Where(q => q.QuestionQuery == resultQuery);
                var so_count = grabQuestions.ToList().Count;
                var grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery).Distinct();
                var msdn_count = grabMSDN.ToList().Count;

                int x = 0;
                int y = 0;
                string new_so_query = "";
                char[] so_delims = {'+'};
                string[] so_query = resultQuery.Split(so_delims);
                foreach (string s in so_query)
                {
                    if (s[x] == s[0])
                        new_so_query += s;
                    else
                        break;
                    x++;
                }

                string new_msdn_query = "";
                char[] msdn_delims = {'%'};
                string[] msdn_query = msdn_resultQuery.Split(msdn_delims);
                foreach (string s in msdn_query)
                {
                    if (s[y] == s[0])
                        new_msdn_query += s;
                    else
                        break;
                    y++;
                }
           


                /*if(so_count == 0)
                    GetStackData(resultQuery);*/

                    

                //var grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery);
                //var msdn_count = grabMSDN.ToList().Count;
                List<Question> finalQuestions = new List<Question>();
                List<MSDN_table> finalMSDN = new List<MSDN_table>();
                string prevMSDNString = "";
                string prevSOstring = "";
                if (so_count == 0)
                {
                    GetStackData(resultQuery);

                
                }

                if (msdn_count == 0)
                {
                    GetMSDNData(resultQuery);

                
                }

                grabQuestions = db.Questions.Where(q => q.QuestionQuery == resultQuery);
                foreach (var item in grabQuestions)
                {
                    if (item.QuestionTitle.Trim() != prevSOstring)
                        finalQuestions.Add(item);

                    prevSOstring = item.QuestionTitle.Trim();
                }


                grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery);
                foreach (var item in grabMSDN)
                {
                    if (item.QueryTitle.Trim() != prevMSDNString)
                        finalMSDN.Add(item);

                    prevMSDNString = item.QueryTitle.Trim();
                }

                List<ResultsViewModel> returnGrabbedList = new List<ResultsViewModel>();
                var grab_all =
                 from m in finalMSDN
                 join q in finalQuestions on m.QuerySearch.Trim() equals q.QuestionQuery.Trim()
                 select new ResultsViewModel { question = q, msdn = m };

            


                return View(grab_all.ToList());
                //.ToPagedList(page ?? 1, 8)
            }
            List<ResultsViewModel> emptyList = new List<ResultsViewModel>();
            return View(emptyList);
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

        public void GetMSDNData(string query)
        {
            string url = "https://services.social.microsoft.com/searchapi/en-US/Msdn?query=" + query + "&amp;maxnumberedpages=5&amp;encoderesults=1&amp;highlightqueryterms=1";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse) request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();

            doc.LoadHtml(objText);

            //dynamic obj = JObject.Parse(objText);
            //string query2 = obj.query.results;

            dynamic data = JObject.Parse(objText);
            JArray test = data.data.results;

            List<string> parsedJArray = new List<string>();
            List<string> parsedTitles = new List<string>();
            
            foreach(var item in test)
            {
                MSDN_Object msdn_result_object = new MSDN_Object();
                msdn_result_object.title = item.Value<string>("title");
                msdn_result_object.description = item.Value<string>("description");
                msdn_result_object.url = item.Value<string>("display_url");
                msdn_result_object.query = resultQuery;
                //need to get rating
                //msdn_result_object.rating = test[idx].First.Value<float>("rating");
                InsertIntoDB(msdn_result_object, "msdn");
                //Console.WriteLine("debug");
            }

            //HtmlDocument doc = new HtmlDocument();
            //HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            /*var response = (HttpWebResponse) request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();

            doc.LoadHtml(objText);*/

            /* HtmlNodeCollection tl = doc.DocumentNode.SelectNodes(@"/query");
            foreach (HtmlNode node in tl)
            {
                Console.WriteLine(node.InnerText);
            }*/



        }

        public void InsertIntoDB(Stack_Object obj, string table)
        {
            string connString =
                System.Configuration.ConfigurationManager.ConnectionStrings[@"SparkHelp"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Error in opening database connection!");
                    return;
                }

                using (
                    SqlCommand cmd =
                        new SqlCommand(
                            "INSERT INTO Questions (QuestionTitle, QuestionLink, QuestionVote, QuestionQuery) VALUES (@qtitle, @qlink, @qvote, @qquery)",
                            conn))
                {
                    SqlParameter sTitle = new SqlParameter();
                    sTitle.ParameterName = "@qtitle";
                    sTitle.Value = obj.title;

                    SqlParameter sLink = new SqlParameter();
                    sLink.ParameterName = "@qlink";
                    sLink.Value = obj.link;

                    SqlParameter sRating = new SqlParameter();
                    sRating.ParameterName = "@qvote";
                    sRating.Value = obj.rating;

                    SqlParameter sQuery = new SqlParameter();
                    sQuery.ParameterName = "@qquery";
                    sQuery.Value = resultQuery;

                    cmd.Parameters.Add(sTitle);
                    cmd.Parameters.Add(sLink);
                    cmd.Parameters.Add(sRating);
                    cmd.Parameters.Add(sQuery);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }


            }
        }

        public void InsertIntoDB(MSDN_Object obj, string table)
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

                using (SqlCommand cmd = new SqlCommand("INSERT INTO MSDN_table (QuerySearch, QueryTitle, QueryDescription, QueryURL) VALUES (@qsearch, @qtitle, @qdesc, @qURL)", conn))
                {
                    SqlParameter mTitle = new SqlParameter();
                    mTitle.ParameterName = "@qtitle";
                    mTitle.Value = obj.title;

                    SqlParameter mURL = new SqlParameter();
                    mURL.ParameterName = "@qURL";
                    mURL.Value = obj.url;

                    SqlParameter mDescription = new SqlParameter();
                    mDescription.ParameterName = "@qdesc";
                    mDescription.Value = obj.description;

                    SqlParameter mQuery= new SqlParameter();
                    mQuery.ParameterName = "@qsearch";
                    mQuery.Value = obj.query;
                    
                    cmd.Parameters.Add(mTitle);
                    cmd.Parameters.Add(mURL);
                    cmd.Parameters.Add(mDescription);
                    cmd.Parameters.Add(mQuery);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void GetStackData(string query)
        {
            var client = new StacManClient(key: "DVtF2taHDlKbZ3TEP8P9Yw((", version: "2.1");

            //var response = client.Sites.GetAll(filter: "default", page: 1, pagesize: 1).Result;
            var response = client.Search.GetMatchesAdvanced(site: "stackoverflow", filter: "default", page: null,
                pagesize: null, fromdate: null, todate: null,
                sort: SearchSort.Relevance, mindate: null, maxdate: null,
                min: null, max: null, order: Order.Desc, q: query, accepted: null,
                answers: null, body: null, closed: null, migrated: null, notice: null,
                nottagged: null, tagged: null,
                title: null, user: null, url: null, views: null, wiki: null).Result;
            foreach (var question in response.Data.Items)
            {
                Console.WriteLine(question.Title);
                Stack_Object stackObject = new Stack_Object();
                stackObject.title = question.Title;
                stackObject.link = question.Link;
                stackObject.rating = question.Score;
                stackObject.query = query;
                
                InsertIntoDB(stackObject, "stackoverflow");

            }
            

            /*if (doc.DocumentNode.SelectNodes("//div [@class=\"result-link\"]") != null)
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
            }*/
        }
    }


}
