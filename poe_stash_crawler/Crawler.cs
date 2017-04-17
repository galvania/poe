using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
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
                    while (o["next_change_id"] != null)
                    {
                        IList<JToken> results = o["stashes"].Children().ToList();
                        results.ToList().Take(20).ToList().ForEach(j => { st.Add(j.ToObject<Stash>());Console.WriteLine(i++); });

                        db.Stashes.AddRange(st);
                        db.SaveChanges();
                        break;
                        content = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs?id=" + o["next_change_id"].ToString());
                        o = JObject.Parse(content);
                    }
                }
            }
            Console.WriteLine();

        }
    }
}
