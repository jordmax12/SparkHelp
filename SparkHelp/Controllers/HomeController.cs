using System;
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



                //GetUnityData(resultQuery);
                var grabQuestions = db.StackOverflows.Where(q => q.QuestionQuery == resultQuery);
                var so_count = grabQuestions.ToList().Count;
                var grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery).Distinct();
                var msdn_count = grabMSDN.ToList().Count;
                var grabCP = db.CodeProjects.Where(c => c.QuestionQuery == resultQuery).Distinct();
                var cp_count = grabCP.ToList().Count;
                var grabUnity = db.Unity3D.Where(u => u.Query == resultQuery).Distinct();
                var U_count = grabUnity.ToList().Count;

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
                List<StackOverflow> finalStack = new List<StackOverflow>();
                List<MSDN_table> finalMSDN = new List<MSDN_table>();
                List<CodeProject> finalCP = new List<CodeProject>();
                List<Unity3D> finalUnity= new List<Unity3D>();
                string prevMSDNString = "";
                string prevSOstring = "";
                string prevCPstring = "";
                string prevUnitystring = "";

                if (so_count == 0)
                {
                    GetStackData(resultQuery);
                }

                if (msdn_count == 0)
                {
                    GetMSDNData(resultQuery);
                }

                if (cp_count == 0)
                {
                    GetCPData(resultQuery);
                }

                //check if unity is checked
                if (U_count == 0)
                {
                    GetUnityData(resultQuery);
                }

                grabQuestions = db.StackOverflows.Where(q => q.QuestionQuery == resultQuery);
                foreach (var item in grabQuestions)
                {
                    if (item.QuestionTitle.Trim() != prevSOstring)
                        finalStack.Add(item);

                    prevSOstring = item.QuestionTitle.Trim();
                }


                grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery);
                foreach (var item in grabMSDN)
                {
                    if (item.QueryTitle.Trim() != prevMSDNString)
                        finalMSDN.Add(item);

                    prevMSDNString = item.QueryTitle.Trim();
                }

                grabCP = db.CodeProjects.Where(c => c.QuestionQuery == resultQuery);
                foreach (var item in grabCP)
                {
                    if (item.Title.Trim() != prevCPstring)
                        finalCP.Add(item);

                    prevCPstring = item.Title.Trim();
                }

                grabUnity = db.Unity3D.Where(u => u.Query == resultQuery);
                foreach (var item in grabUnity)
                {
                    if (item.Title.Trim() != prevUnitystring)
                        finalUnity.Add(item);

                    prevUnitystring = item.Title.Trim();
                }

                var grab_all =
                 from m in finalMSDN
                 join s in finalStack on m.QuerySearch.Trim() equals s.QuestionQuery.Trim()
                 join c in finalCP on m.QuerySearch.Trim() equals c.QuestionQuery.Trim()
                 join u in finalUnity on m.QuerySearch.Trim() equals u.Query.Trim()
                 select new ResultsViewModel { stack = s, msdn = m, CP = c, unity = u};

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
                            "INSERT INTO StackOverflow (QuestionTitle, QuestionLink, QuestionVote, QuestionQuery) VALUES (@qtitle, @qlink, @qvote, @qquery)",
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

        public void InsertIntoDB(CP_Object obj)
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

                using ( SqlCommand cmd =new SqlCommand(
                    "INSERT INTO CodeProject (Title, Link, Rating, Votes, Summary, QuestionQuery) VALUES (@title, @link, @rating, @votes, @summary, @query)",
                            conn))
                {
                    SqlParameter cTitle = new SqlParameter();
                    cTitle.ParameterName = "@title";
                    cTitle.Value = obj.title;

                    SqlParameter cLink = new SqlParameter();
                    cLink.ParameterName = "@link";
                    cLink.Value = obj.link;

                    SqlParameter cRating = new SqlParameter();
                    cRating.ParameterName = "@rating";
                    cRating.Value = obj.rating;

                    SqlParameter cVotes = new SqlParameter();
                    cVotes.ParameterName = "@votes";
                    cVotes.Value = obj.votes;

                    SqlParameter cSummary = new SqlParameter();
                    cSummary.ParameterName = "@summary";
                    cSummary.Value = obj.summary;

                    SqlParameter cQuery = new SqlParameter();
                    cQuery.ParameterName = "@query";
                    cQuery.Value = obj.query;

                    cmd.Parameters.Add(cTitle);
                    cmd.Parameters.Add(cLink);
                    cmd.Parameters.Add(cRating);
                    cmd.Parameters.Add(cVotes);
                    cmd.Parameters.Add(cSummary);
                    cmd.Parameters.Add(cQuery);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void InsertIntoDB(Unity_Object obj)
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

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Unity3D (Title, Link, Snippet, Query, CheckedAnswer) VALUES (@title, @link, @snippet, @query, @canswer)",
                            conn))
                {
                    SqlParameter cTitle = new SqlParameter();
                    cTitle.ParameterName = "@title";
                    cTitle.Value = obj.title;

                    SqlParameter cLink = new SqlParameter();
                    cLink.ParameterName = "@link";
                    cLink.Value = obj.link;

                    SqlParameter uSnippet = new SqlParameter();
                    uSnippet.ParameterName = "@snippet";
                    uSnippet.Value = obj.snippet;

                    SqlParameter cQuery = new SqlParameter();
                    cQuery.ParameterName = "@query";
                    cQuery.Value = obj.query;

                    SqlParameter uCAnswer = new SqlParameter();
                    uCAnswer.ParameterName = "@canswer";
                    uCAnswer.Value = "null";

                    if (obj.checkedAnswer != null)
                    {
                        uCAnswer.ParameterName = "@canswer";
                        obj.checkedAnswer = Truncate(obj.checkedAnswer, 4000);
                        uCAnswer.Value = obj.checkedAnswer;
                    }




                    cmd.Parameters.Add(cTitle);
                    cmd.Parameters.Add(cLink);
                    cmd.Parameters.Add(uSnippet);
                    cmd.Parameters.Add(cQuery);
                    cmd.Parameters.Add(uCAnswer);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void GetMSDNData(string query)
        {
            string url = "https://services.social.microsoft.com/searchapi/en-US/Msdn?query=" + query + "&amp;maxnumberedpages=5&amp;encoderesults=1&amp;highlightqueryterms=1";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();

            doc.LoadHtml(objText);

            //dynamic obj = JObject.Parse(objText);
            //string query2 = obj.query.results;

            dynamic data = JObject.Parse(objText);
            JArray test = data.data.results;

            List<string> parsedJArray = new List<string>();
            List<string> parsedTitles = new List<string>();

            foreach (var item in test)
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
        }

        public void GetCPData(string query)
        {
            string url = "http://www.codeproject.com/search.aspx?q=" + query + "&x=0&y=0&sbo=kw&pgsz=10";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            
            int idx = 0;
            if (doc.DocumentNode.SelectNodes("//div [@class=\"hover-container content-list-item\"]") != null)
            {
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div [@class=\"entry\"]"))
                {
                    CP_Object CP = new CP_Object();
                    CP.title = node.SelectSingleNode(".//span [@class=\"title\"]").InnerText;
                    CP.link = "www.codeproject.com/" +
                        node.SelectSingleNode(".//a").Attributes["href"].Value;
                    char[] delimCharFirst = { '=' };
                    string[] grabRating =
                        node.SelectSingleNode(".//div [@class=\"nowrap rating-stars-small\"]")
                            .OuterHtml.Split(delimCharFirst);
                    string getRatingContent = grabRating[2].TrimStart();
                    char[] delimCharSecond = { ' ' };
                    char[] delimCharThird = { '(' };
                    string[] ratingAfterSplit = getRatingContent.Split(delimCharSecond);
                    string[] votesSplit = ratingAfterSplit[1].Split(delimCharThird);
                    CP.rating = float.Parse(ratingAfterSplit[0]);
                    CP.votes = Convert.ToInt32(votesSplit[1]);
                    CP.summary = node.SelectSingleNode(".//div [@class=\"summary\"]").InnerText;
                    CP.query = query;
                    InsertIntoDB(CP);
                }
                
            }
        }

        public void GetUnityData(string query)
        {
            string url = "https://www.googleapis.com/customsearch/v1?key=AIzaSyDeuOYCUjA22jGcokluJUI36K6XE7Ixs1M&cx=003540131153181508748:oa0em0h_kv0&q=" + query + "&alt=json";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();

            doc.LoadHtml(objText);

            //dynamic obj = JObject.Parse(objText);
            //string query2 = obj.query.results;

            dynamic data = JObject.Parse(objText);
            JArray test = data.items;

            if (test != null)
            {
                foreach (var item in test)
                {
                    Unity_Object UO = new Unity_Object();
                    UO.title = item.Value<string>("title");
                    UO.link = item.Value<string>("link");
                    UO.snippet = item.Value<string>("snippet");
                    UO.query = query;


                    HtmlWeb sub_web = new HtmlWeb();
                    HtmlDocument sub_doc = sub_web.Load(UO.link);



                    if (sub_doc.DocumentNode.SelectNodes("//div [@class=\"answer full\"]") != null)
                    {
                        UO.checkedAnswer =
                            sub_doc.DocumentNode.SelectSingleNode("//div [@class=\"answer full\"]")
                                .SelectSingleNode(".//div [@class=\"answer-body\"]")
                                .InnerText.TrimStart().TrimEnd();

                    }

                    InsertIntoDB(UO);
                    Console.WriteLine("debug");
                }
            }


        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }


}
