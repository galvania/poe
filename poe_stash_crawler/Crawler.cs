using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace poe_stash_crawler
{
    public class Crawler
    {
        List<Stash> st = new List<Stash>();

        public Crawler()
        {
            string content;
            using (var client = new WebClient())
            {
                using (var db = new CFContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    content = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs");
                    int i = 0;
                    var o = JObject.Parse(content);
                    object stlock= new object();
                    List<JToken> results;
                    while (o["next_change_id"] != null)
                    {
                        new Task(()=> {
                            lock (stlock)
                            {
                                results = o["stashes"].Children().ToList();
                                results.ToList().ForEach(j => { st.Add(j.ToObject<Stash>()); Console.WriteLine(i++); });
                            }
                        }).Start();

                        content = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs?id=" + o["next_change_id"].ToString());
                        o = JObject.Parse(content);
                        
                    }
                }
            }
            Console.WriteLine();

        }


    }
}
