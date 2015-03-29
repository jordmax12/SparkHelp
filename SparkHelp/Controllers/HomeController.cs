using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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

        public ActionResult Index(string queried, int? page, string Stack, string MSDN, string W3, string CodeProject, string Unity3D)
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
                    try
                    {
                        if (s[i] == s[0])
                            resultQuery += s;
                        else
                            resultQuery += "+" + s;
                    }
                    catch
                    {
                        resultQuery += "+" + s;
                    }
                    i++;
                }

                //STACKOVERFLOW
                List<StackOverflow> finalStack = new List<StackOverflow>();
                if (Stack == "true" && Stack != null)
                {
                    var grabQuestions = db.StackOverflows.Where(q => q.QuestionQuery == resultQuery);
                    var so_count = grabQuestions.ToList().Count;
                    int max_count = 10;

                    string prevSOstring = "";

                    if (so_count == 0)
                    {
                        GetStackData(resultQuery, max_count);
                    }

                    grabQuestions = db.StackOverflows.Where(q => q.QuestionQuery == resultQuery);
                    foreach (var item in grabQuestions)
                    {
                        if (item.QuestionTitle.Trim() != prevSOstring)
                            finalStack.Add(item);

                        prevSOstring = item.QuestionTitle.Trim();
                    }
                } else if (Stack == null)
                {
                    StackOverflow stackObject = new StackOverflow();
                    stackObject.QuestionTitle = "null";
                    stackObject.QuestionLink = "null";
                    stackObject.QuestionQuery = resultQuery;
                    finalStack.Add(stackObject);
                }


                //MSDN
                List<MSDN_table> finalMSDN = new List<MSDN_table>();
                if (MSDN == "true" && MSDN != null)
                {
                    var grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery).Distinct();
                    var msdn_count = grabMSDN.ToList().Count;
                    int max_count = 10;
                    
                    string prevMSDNString = "";
                    if (msdn_count == 0)
                    {
                        GetMSDNData(resultQuery, max_count);
                    }

                    grabMSDN = db.MSDN_table.Where(m => m.QuerySearch == resultQuery);
                    foreach (var item in grabMSDN)
                    {
                        if (item.QueryTitle.Trim() != prevMSDNString)
                            finalMSDN.Add(item);

                        prevMSDNString = item.QueryTitle.Trim();
                    }
                } else if (MSDN == null)
                {
                    MSDN_table MSDN_temp = new MSDN_table();
                    MSDN_temp.QueryTitle = "null";
                    MSDN_temp.QuerySearch = resultQuery;
                    MSDN_temp.QueryDescription = "null";
                    MSDN_temp.QueryURL = "null";
                    finalMSDN.Add(MSDN_temp);
                }

                //CP
                List<CodeProject> finalCP = new List<CodeProject>();
                if (CodeProject == "true" && CodeProject != null)
                {
                    var grabCP = db.CodeProjects.Where(c => c.QuestionQuery == resultQuery).Distinct();
                    var cp_count = grabCP.ToList().Count;
                    int max_count = 8;
                    string prevCPstring = "";
                    if (cp_count == 0)
                    {
                        GetCPData(resultQuery, max_count);
                    }

                    grabCP = db.CodeProjects.Where(c => c.QuestionQuery == resultQuery);
                    foreach (var item in grabCP)
                    {
                        if (item.Title.Trim() != prevCPstring)
                            finalCP.Add(item);

                        prevCPstring = item.Title.Trim();
                    }
                } else if (CodeProject == null)
                {
                    CodeProject CP = new CodeProject();
                    CP.Title = "null";
                    CP.Link = "null";
                    CP.Rating = 0.0f;
                    CP.Votes = 0;
                    CP.Summary = "null";
                    CP.QuestionQuery = resultQuery;
                    finalCP.Add(CP);
                }

                //UNITY
                List<Unity3D> finalUnity = new List<Unity3D>();
                if (Unity3D == "true" && Unity3D != null)
                {
                    var grabUnity = db.Unity3D.Where(u => u.Query == resultQuery).Distinct();
                    var U_count = grabUnity.ToList().Count;
                    int max_count = 8;
                    string prevUnitystring = "";
                    if (U_count == 0)
                    {
                        GetUnityData(resultQuery, max_count);
                    }

                    grabUnity = db.Unity3D.Where(u => u.Query == resultQuery);
                    foreach (var item in grabUnity)
                    {
                        if (item.Title.Trim() != prevUnitystring)
                            finalUnity.Add(item);

                        prevUnitystring = item.Title.Trim();
                    }
                } else if (Unity3D == null)
                {
                    Unity3D UO = new Unity3D();
                    UO.Title = "null";
                    UO.Link = "null";
                    UO.Snippet = "null";
                    UO.Query = resultQuery;
                    UO.CheckedAnswer = "null";
                    finalUnity.Add(UO);
                }



                //W3
                List<W3> finalW3 = new List<W3>();
                if (W3 == "true" && W3 != null)
                {
                    var grabW3 = db.W3.Where(w => w.Query == resultQuery).Distinct();
                    var W_count = grabW3.ToList().Count;
                    int max_count = 8;
                    string prevW3string = "";
                    if (W_count == 0)
                    {
                        GetW3Data(resultQuery, max_count);
                    }

                    grabW3 = db.W3.Where(w => w.Query == resultQuery);
                    foreach (var item in grabW3)
                    {
                        if (item.Title.Trim() != prevW3string)
                            finalW3.Add(item);

                        prevW3string = item.Title.Trim();
                    }
                } else if (W3 == null)
                {
                    W3 W3_temp = new W3();
                    W3_temp.Title = "null";
                    W3_temp.Link = "null";
                    W3_temp.Snippet = "null";
                    W3_temp.Query = resultQuery;
                    finalW3.Add(W3_temp);
                }




                if (finalMSDN.Count != 0)
                {
                        var grab_all =
                            from m in finalMSDN
                            join s in finalStack on m.QuerySearch.Trim() equals s.QuestionQuery.Trim()
                            join c in finalCP on m.QuerySearch.Trim() equals c.QuestionQuery.Trim()
                            join u in finalUnity on m.QuerySearch.Trim() equals u.Query.Trim()
                            join w in finalW3 on m.QuerySearch.Trim() equals w.Query.Trim()
                            select new ResultsViewModel {stack = s, msdn = m, CP = c, unity = u, w3 = w};

                        return View(grab_all.ToList());
                    
                }
                else
                {
                      var grab_all =
                        from s in finalStack
                        join c in finalCP on s.QuestionQuery.Trim() equals c.QuestionQuery.Trim()
                        join u in finalUnity on s.QuestionQuery.Trim() equals u.Query.Trim()
                        join w in finalW3 on s.QuestionQuery.Trim() equals w.Query.Trim()
                        select new ResultsViewModel { stack = s, CP = c, unity = u, w3 = w };

                    return View(grab_all.ToList());
                }


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

        
       

        public void InsertIntoDB(StackOverflow obj, string table)
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
                            "INSERT INTO StackOverflow (QuestionTitle, QuestionLink, QuestionVote, QuestionQuery, QuestionText) VALUES (@qtitle, @qlink, @qvote, @qquery, @qtext)",
                            conn))
                {
                    SqlParameter sTitle = new SqlParameter();
                    sTitle.ParameterName = "@qtitle";
                    sTitle.Value = obj.QuestionTitle;

                    SqlParameter sLink = new SqlParameter();
                    sLink.ParameterName = "@qlink";
                    sLink.Value = obj.QuestionLink;

                    SqlParameter sRating = new SqlParameter();
                    sRating.ParameterName = "@qvote";
                    sRating.Value = obj.QuestionVote;

                    SqlParameter sQuery = new SqlParameter();
                    sQuery.ParameterName = "@qquery";
                    sQuery.Value = resultQuery;

                    SqlParameter sText = new SqlParameter();
                    if (obj.QuestionText != null)
                    {
                        
                        sText.ParameterName = "@qtext";
                        sText.Value = obj.QuestionText;
                    }
                    else
                    {
                        sText.ParameterName = "@qtext";
                        sText.Value = "null value";
                    }

                    

                    cmd.Parameters.Add(sTitle);
                    cmd.Parameters.Add(sLink);
                    cmd.Parameters.Add(sRating);
                    cmd.Parameters.Add(sQuery);
                    cmd.Parameters.Add(sText);
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
                    obj.title = Truncate(obj.title, 200);
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
                    obj.summary = Truncate(obj.summary, 4000);
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
                    SqlParameter uTitle = new SqlParameter();
                    uTitle.ParameterName = "@title";
                    uTitle.Value = obj.title;

                    SqlParameter uLink = new SqlParameter();
                    uLink.ParameterName = "@link";
                    uLink.Value = obj.link;

                    SqlParameter uSnippet = new SqlParameter();
                    uSnippet.ParameterName = "@snippet";
                    uSnippet.Value = obj.snippet;

                    SqlParameter uQuery = new SqlParameter();
                    uQuery.ParameterName = "@query";
                    uQuery.Value = obj.query;

                    SqlParameter uCAnswer = new SqlParameter();
                    uCAnswer.ParameterName = "@canswer";
                    uCAnswer.Value = "null";

                    if (obj.checkedAnswer != null)
                    {
                        uCAnswer.ParameterName = "@canswer";
                        obj.checkedAnswer = Truncate(obj.checkedAnswer, 4000);
                        uCAnswer.Value = obj.checkedAnswer;
                    }

                    cmd.Parameters.Add(uTitle);
                    cmd.Parameters.Add(uLink);
                    cmd.Parameters.Add(uSnippet);
                    cmd.Parameters.Add(uQuery);
                    cmd.Parameters.Add(uCAnswer);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void InsertIntoDB(W3_Object obj)
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
                    "INSERT INTO W3 (Title, Link, Snippet, Query) VALUES (@title, @link, @snippet, @query)",
                            conn))
                {
                    SqlParameter wTitle = new SqlParameter();
                    wTitle.ParameterName = "@title";
                    wTitle.Value = obj.title;

                    SqlParameter wLink = new SqlParameter();
                    wLink.ParameterName = "@link";
                    wLink.Value = obj.link;

                    SqlParameter wSnippet = new SqlParameter();
                    wSnippet.ParameterName = "@snippet";
                    wSnippet.Value = obj.snippet;

                    SqlParameter wQuery = new SqlParameter();
                    wQuery.ParameterName = "@query";
                    wQuery.Value = obj.query;

                    cmd.Parameters.Add(wTitle);
                    cmd.Parameters.Add(wLink);
                    cmd.Parameters.Add(wSnippet);
                    cmd.Parameters.Add(wQuery);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void GetMSDNData(string query, int max)
        {
            string url = "https://services.social.microsoft.com/searchapi/en-US/Msdn?query=" + query +
                         "&amp;maxnumberedpages=5&amp;encoderesults=1&amp;highlightqueryterms=1";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            int idx = 0;

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var objText = reader.ReadToEnd();

                doc.LoadHtml(objText);

                dynamic data = JObject.Parse(objText);
                JArray test = data.data.results;

                if (test == null)
                {
                    MSDN_Object msdn_result_object = new MSDN_Object();
                    msdn_result_object.title = "null";
                    msdn_result_object.description = "null";
                    msdn_result_object.url = "null";
                    msdn_result_object.query = resultQuery;
                    InsertIntoDB(msdn_result_object, "msdn");

                }
                else
                {
                    foreach (var item in test)
                    {
                        if (idx <= max)
                        {
                            MSDN_Object msdn_result_object = new MSDN_Object();
                            msdn_result_object.title = item.Value<string>("title");
                            msdn_result_object.description = item.Value<string>("description");
                            msdn_result_object.url = item.Value<string>("display_url");
                            msdn_result_object.query = resultQuery;
                            InsertIntoDB(msdn_result_object, "msdn");
                            idx++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("error");
            }
        }

        public void GetStackData(string query, int max)
        {
            var client = new StacManClient(key: "DVtF2taHDlKbZ3TEP8P9Yw((", version: "2.1");
            int idx = 0;

            var response = client.Search.GetMatchesAdvanced(site: "stackoverflow", filter: "default", page: null,
                pagesize: null, fromdate: null, todate: null,
                sort: SearchSort.Relevance, mindate: null, maxdate: null,
                min: null, max: null, order: Order.Desc, q: query, accepted: null,
                answers: null, body: null, closed: null, migrated: null, notice: null,
                nottagged: null, tagged: null,
                title: null, user: null, url: null, views: null, wiki: null).Result;

            if (response.Data.Items.Count() == 0)
            {
                StackOverflow stackObject = new StackOverflow();
                stackObject.QuestionTitle = "null";
                stackObject.QuestionLink = "null";
                stackObject.QuestionText = "null";
                stackObject.QuestionVote = 0;
                stackObject.QuestionQuery = query;

                InsertIntoDB(stackObject, "stackoverflow");
            }
            else
            {
                foreach (var question in response.Data.Items)
                {
                    if (idx <= max)
                    {
                        StackOverflow stackObject = new StackOverflow();
                        stackObject.QuestionTitle = question.Title;
                        stackObject.QuestionLink = question.Link;
                        stackObject.QuestionVote = question.Score;
                        stackObject.QuestionQuery = query;

                        HtmlWeb web = new HtmlWeb();
                        HtmlDocument doc = web.Load(stackObject.QuestionLink);
                        if (doc.DocumentNode.SelectNodes("//div [@class=\"question\"]") != null)
                        {
                            var question_text =
                                doc.DocumentNode.SelectSingleNode("//div [@class=\"question\"]")
                                    .SelectSingleNode(".//div [@class=\"post-text\"]")
                                    .InnerText;


                            if (question_text.Count() >= 147)
                            {
                                question_text = Truncate(question_text, 147);
                                question_text += "...";
                            }

                            stackObject.QuestionText = question_text.Trim();
                        }

                        InsertIntoDB(stackObject, "stackoverflow");
                        idx++;
                    }
                }
            }



        }

        public void GetCPData(string query, int max)
        {
            string url = "http://www.codeproject.com/search.aspx?q=" + query + "&x=0&y=0&sbo=kw&pgsz=10";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            
            int idx = 0;
            if (doc.DocumentNode.SelectNodes("//div [@class=\"hover-container content-list-item\"]") != null)
            {
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div [@class=\"entry\"]"))
                {
                    if (idx <= max)
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
                        try
                        {
                            float temp = float.Parse(ratingAfterSplit[0]);
                            CP.rating = temp;
                        }
                        catch
                        {
                            CP.rating = 0.0f;
                        }
                        try
                        {
                            CP.votes = Convert.ToInt32(votesSplit[1]);
                        }
                        catch
                        {
                            CP.votes = 0;
                        }
                        CP.summary = node.SelectSingleNode(".//div [@class=\"summary\"]").InnerText;
                        CP.query = query;
                        InsertIntoDB(CP);
                        idx++;
                    }
                }

            }
            else
            {
                CP_Object CP = new CP_Object();
                CP.title = "null";
                CP.link = "null";
                CP.rating = 0.0f;
                CP.votes = 0;
                CP.summary = "null";
                CP.query = query;
                InsertIntoDB(CP);
            }
        }

        public void GetUnityData(string query, int max)
        {
            bool success = false;
            string url = "https://www.googleapis.com/customsearch/v1?key=AIzaSyARl7587QeTqdMqP1rFKVj2UuNlrtoFsak&cx=003540131153181508748:oa0em0h_kv0&userIp=104.45.129.178&q=" + query + "&alt=json";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            int idx = 0;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var objText = reader.ReadToEnd();
                success = true;
                doc.LoadHtml(objText);

                dynamic data = JObject.Parse(objText);
                JArray test = data.items;

                if (test != null)
                {
                    foreach (var item in test)
                    {
                        if (idx <= max)
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
                            else
                            {
                                UO.checkedAnswer = "null";
                            }

                            InsertIntoDB(UO);
                            idx++;
                        }
                    }
                }
                else
                {
                    Unity_Object UO = new Unity_Object();
                    UO.title = "null";
                    UO.link = "null";
                    UO.snippet = "null";
                    UO.query = query;
                    UO.checkedAnswer = "null";
                    InsertIntoDB(UO);

                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    success = false;
                    Unity_Object UO = new Unity_Object();
                    UO.title = "null";
                    UO.link = "null";
                    UO.snippet = "null";
                    UO.query = query;
                    UO.checkedAnswer = "null";
                    InsertIntoDB(UO);
                }
            }


            Console.WriteLine(  "debug");


        }

        public void GetW3Data(string query, int max)
        {
            bool success = false;
            string url = "https://www.googleapis.com/customsearch/v1?key=AIzaSyARl7587QeTqdMqP1rFKVj2UuNlrtoFsak&cx=003540131153181508748:rfllse6yorm&q=" + query + "&alt=json";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            int idx = 0;

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var objText = reader.ReadToEnd();

                doc.LoadHtml(objText);
                success = true;

                dynamic data = JObject.Parse(objText);
                JArray test = data.items;

                if (test != null)
                {
                    foreach (var item in test)
                    {
                        if (idx <= max)
                        {
                            W3_Object W3 = new W3_Object();
                            W3.title = item.Value<string>("title");
                            W3.link = item.Value<string>("link");
                            W3.snippet = item.Value<string>("snippet");
                            W3.query = query;

                            InsertIntoDB(W3);
                            idx++;
                        }
                    }
                }
                else
                {
                    W3_Object W3 = new W3_Object();
                    W3.title = "null";
                    W3.link = "null";
                    W3.snippet = "null";
                    W3.query = query;
                    InsertIntoDB(W3);

                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    success = false;
                    W3_Object W3 = new W3_Object();
                    W3.title = "null";
                    W3.link = "null";
                    W3.snippet = "null";
                    W3.query = query;
                    InsertIntoDB(W3);
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