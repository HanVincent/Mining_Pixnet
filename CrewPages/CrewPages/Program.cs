using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace CrewPages
{
    class Program
    {
        static void Main(string[] args)
        {
            //pixnet API，近期熱門文章，24: 數位生活
            //https://emma.pixnet.cc/mainpage/blog/categories/how_weekly/24?page=1&per_page=3&api_version=2&format=json
            //https://emma.pixnet.cc/mainpage/blog/categories/latest/24?page=1&per_page=3&api_version=2&format=json

            //先跑近期熱門，之後再跑最新文章
            int page = 1;
            while (true)
            {
                string j_content = crewWeb(@"https://emma.pixnet.cc/mainpage/blog/categories/latest/24?page=" + page + "&per_page=100&api_version=2&format=json");//近期熱門文章，未parse

                JObject jp_hot_weekly = JObject.Parse(j_content);//做parsing，以便可以直接抓取網址和User ID
                if (jp_hot_weekly["error"] == null) break; //當沒有下一頁的時候，回傳false

                StreamWriter sw = new StreamWriter(@"./latest/latest_" + page + ".json");//將最近熱門的寫成檔案
                sw.Write(jp_hot_weekly);
                sw.Close();

                JArray j_articles = (JArray)jp_hot_weekly["articles"];//將所有文章轉換成陣列
                if (j_articles == null) break;

                for (int i = 0; i < j_articles.Count; i++) //洗出各使用者的所有文章
                {
                    /*將所有熱門文章的user抓取出來並用API找到他所有文章，並寫入users資料夾內，以user名稱為檔名*/
                    string name = j_articles[i]["user"]["name"].ToString();
                    Console.WriteLine(name);

                    int detailPage = 1;
                    while (true)
                    {
                        string j_user_all = crewWeb(@"https://emma.pixnet.cc/blog/articles?user=" + name + "&page=" + detailPage + "&per_page=100&format=json");//特定使用者的所有文章
                        if (j_user_all == "") 
                        { 
                            Console.WriteLine("j_user_all=\"\"");
                            continue; //try rerun
                        }
                        
                        JObject jp_user_all_articles = JObject.Parse(j_user_all);

                        sw = new StreamWriter(@"./users/" + name + "_" + detailPage + ".json");
                        sw.Write(jp_user_all_articles);
                        sw.Close();

                        if (!Directory.Exists(@"./articles/" + name + "/"))
                            Directory.CreateDirectory(@"./articles/" + name + "/");

                        /*-----------------------------------------------------------------*/
                        /*抓取該user的article link並去找到該網頁抓出所有html，寫檔*/
                        JArray ja_user_article = (JArray)jp_user_all_articles["articles"];//將該使用者的所有文章轉陣列
                        if (ja_user_article == null) break;

                        for (int j = 0; j < ja_user_article.Count; j++)
                        {
                            string article_id = ja_user_article[j]["id"].ToString();
                            Console.WriteLine(name + " " + article_id);
                            if (File.Exists(@"./articles/" + name + "/" + article_id + ".json")) //文章已經抓過則跳過，省時間
                                continue;

                            string htmlContent = crewWeb(ja_user_article[j]["link"].ToString());//抓取該文章HTML架構

                            if (htmlContent.Equals("")) continue;

                            sw = new StreamWriter(@"./articles/" + name + "/" + article_id + ".json");
                            sw.Write(htmlContent);
                            sw.Close();
                        }
                        /*------------------------------------------------------------------*/
                        detailPage++;
                    }//while end
                }//for end
                page++;
            }//while end


        }//Main end

        /*主要爬網站的function，不管什麼網頁，都會把內容取出。
         *可套用在pixnet的API或熱門文章的網址
         */
        public static string crewWeb(string url)
        {
            /*連結網站*/
            WebRequest myRequest = WebRequest.Create(url);//連結要取得的HTML網頁
            myRequest.Timeout = 60000;
            myRequest.Method = "GET";
            WebResponse myResponse;
            try
            {
                myResponse = myRequest.GetResponse();//取得回應

                /*索取網頁內容*/
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);//Streamreader讀取回覆
                string content = sr.ReadToEnd();//將全文轉成string
                sr.Close();
                myResponse.Close();//關掉WebRespons

                return content;
            }
            catch (Exception e)//有時候會crash，天知道為何，馬的
            {
                return "";
            }
        }
    }
}
