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
using System.Data.SqlClient;
using System.Data;
using System.Collections.Concurrent;
using System.Threading;

namespace poe_stash_crawler
{
    public class Crawler
    {
        List<Stash> st = new List<Stash>();
        static int i = 0;
        ConcurrentQueue<List<Stash>> stashqueue = new ConcurrentQueue<List<Stash>>();
        ConcurrentQueue<JObject> jsonData = new ConcurrentQueue<JObject>();
        volatile string next_change_id = " ";
        volatile bool stop = false;
        public Crawler()
        {
            Task downloadTask = new Task(() =>
              {
                  Console.WriteLine("DownloadTask started");
                  using (var client = new WebClient())
                  {
                      JObject jo;
                      string contents;
                      while ((next_change_id ?? "") != "" && !stop)
                      {
                          contents = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs?id=" + next_change_id);
                          jo = JObject.Parse(contents);
                          next_change_id = jo["next_change_id"].ToString();
                          jsonData.Enqueue(jo);
                          while (stashqueue.Count > 5000) Thread.Sleep(5000);
                      }
                      stop = true;
                  }
              });

            //Task databaseInsertTask = new Task(() =>
            //  {
            //      using (SqlConnection dbConnection = new SqlConnection("Data Source=KYOTO;Initial Catalog=poe;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            //      {
            //          JObject j;
            //          Stash s;
            //          while (!stop)
            //              if (jsonData.TryDequeue(out j))
            //              {
            //                  s = j.ToObject<Stash>();
            //                  insertStash(dbConnection, s);
            //                  insertStashItems(dbConnection, s);
            //              }
            //              else
            //                  Thread.Sleep(500);
            //      }
            //  });
            downloadTask.Start();
            startConversionTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            startInsertTask();
            string input = "";
            while (input != "END")
            {
                input = Console.ReadLine();
                switch (input)
                {
                    case "END":
                        stop = true;
                        break;
                    case "i":
                        startConversionTask();
                        startInsertTask();
                        break;
                    case "ct":
                        startConversionTask();
                        break;
                    case "it":
                        startInsertTask();
                        break;
                    case "":
                        Console.WriteLine($"jsonData {jsonData.Count,-10} stashqueue {stashqueue.Count,-10} i {i,-10} ");
                        break;

                }
            }
            Task.WaitAll();
            Console.WriteLine(next_change_id);
            Console.ReadLine();

            //string content;
            //using (var client = new WebClient())
            //{

            //    using (SqlConnection dbConnection = new SqlConnection("Data Source=KYOTO;Initial Catalog=poe;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            //    {

            //        content = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs");
            //        var o = JObject.Parse(content);
            //        object stlock = new object();
            //        List<JToken> results;
            //        dbConnection.Open();
            //        while (o["next_change_id"] != null)
            //        {

            //            results = o["stashes"].Children().ToList();
            //            results.ToList().ForEach(j => { st.Add(j.ToObject<Stash>()); stashqueue.Enqueue(j.ToObject<Stash>()); });
            //            content = client.DownloadString("http://www.pathofexile.com/api/public-stash-tabs?id=" + o["next_change_id"].ToString());
            //            o = JObject.Parse(content);
            //            foreach (var s in st)
            //            {
            //                insertStash(dbConnection, s);
            //                insertStashItems(dbConnection, s);

            //            }
            //            st.Clear();
            //        }
            //        dbConnection.Close();

            //    }
            //}
            //Console.WriteLine();

        }

        private void startInsertTask()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("InsertTask started");

                using (SqlConnection dbConnection = new SqlConnection("Data Source=KYOTO;Initial Catalog=poe;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                {
                    dbConnection.Open();
                    Stash s;
                    while (!stop)
                        if (stashqueue.Count > 0 && stashqueue.TryDequeue(out s))
                        {
                            insertStash(dbConnection, s);
                            insertStashItems(dbConnection, s);
                        }
                        else
                            Thread.Sleep(500);
                }
            });
        }

        private void startConversionTask()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("ConversionTask started");
                JObject jo;
                List<JToken> results;
                List<Stash> stashList = new List<Stash>();
                while (!stop)
                    if (jsonData.Count > 0 && jsonData.TryDequeue(out jo))
                    {
                        results = jo["stashes"].Children().ToList();
                        results.ToList().ForEach(j => {
                            //stashqueue.Enqueue(j.ToObject<Stash>());
                            stashList.Add(j.ToObject<Stash>());

                        });
                    }
                    else
                        Thread.Sleep(500);
            });
        }

        private static void insertStashItems(SqlConnection dbConnection, Stash s)
        {
            if ((s.accountName ?? "") == "") return;
            SqlCommand itemCommand;
            foreach (var stashItem in s.items)
            {
                string query;
                int iID;
                insertItem(dbConnection, s, out itemCommand, stashItem, out query, out iID);
                insertProperties(dbConnection, stashItem.properties, iID, false);
                insertProperties(dbConnection, stashItem.additionalProperties, iID, true);
                insertStringArrays(dbConnection, stashItem.explicitMods, iID, "explicitMods");
                insertStringArrays(dbConnection, stashItem.implicitMods, iID, "implicitMods");
                insertStringArrays(dbConnection, stashItem.enchantMods, iID, "enchantMods");
                insertStringArrays(dbConnection, stashItem.craftedMods, iID, "craftedMods");
                insertStringArrays(dbConnection, stashItem.flavourText, iID, "flavourText");
                insertStringArrays(dbConnection, stashItem.utilityMods, iID, "utilityMods");
                insertStringArrays(dbConnection, stashItem.cosmeticMods, iID, "cosmeticMods");
                insertSockets(dbConnection, stashItem, iID);
                insertRequirements(dbConnection, stashItem, iID);
            }
        }

        private static void insertStringArrays(SqlConnection dbConnection, List<String> stashItem, int iID, string tables)
        {
            string commands = "";
            if (stashItem == null) return;
            string query2 = $@"INSERT INTO [dbo].[{tables}]
           ([value]
           ,[Item_id])
     VALUES
           (@value
           ,@Item_id)";
            SqlCommand socketCommand = new SqlCommand(query2, dbConnection);

            foreach (var item in stashItem)
            {
                socketCommand = new SqlCommand(query2, dbConnection);

                socketCommand.Parameters.AddWithValue("@value", item);
                socketCommand.Parameters.AddWithValue("@Item_id", iID);
                commands += socketCommand.CommandText;
            }
            socketCommand.CommandText = commands;
            if (commands == "") return;
            socketCommand.ExecuteNonQuery();

        }
        private static void insertSockets(SqlConnection dbConnection, Item stashItem, int iID)
        {
            string query = @"INSERT INTO [dbo].[Sockets]
           ([group]
           ,[attr]
           ,[Item_id])
     VALUES
           (@group
           ,@attr
           ,@Item_id)";

            if (stashItem.additionalProperties != null)
            {
                SqlCommand socketCommand = new SqlCommand(query, dbConnection);
                string command = "";
                foreach (var item in stashItem.sockets)
                {
                    socketCommand = new SqlCommand(query, dbConnection);
                    socketCommand.Parameters.AddWithValue("@group", item.group);
                    socketCommand.Parameters.AddWithValue("@attr", item.attr);
                    socketCommand.Parameters.AddWithValue("@Item_id", iID);
                    command += socketCommand;
                }
                if (command == "") return;


                socketCommand.CommandText = command;
                    socketCommand.ExecuteNonQuery();
                
            }

        }
        private static void insertItem(SqlConnection dbConnection, Stash s, out SqlCommand itemCommand, Item stashItem, out string query, out int iID)
        {
            query = @"INSERT INTO [dbo].[Items] 
           ([itemID]
           ,[verified]
           ,[w]
           ,[h]
           ,[ilvl]
           ,[icon]
           ,[league]
           ,[name]
           ,[typeLine]
           ,[identified]
           ,[corrupted]
           ,[lockedToCharacter]
           ,[note]
           ,[frameType]
           ,[x]
           ,[y]
           ,[inventoryId]
           ,[secDescrText]
           ,[descrText]
           ,[artFilename]
           ,[duplicated]
           ,[maxStackSize]
           ,[stackSize]
           ,[talismanTier]
           ,[support]
           ,[prophecyDiffText]
           ,[prophecyText]
           ,[isRelic]
           ,[Stash_id])
     OUTPUT INSERTED.iID VALUES
           (@itemID
           ,@verified
           ,@w
           ,@h
           ,@ilvl
           ,@icon
           ,@league
           ,@name
           ,@typeLine
           ,@identified
           ,@corrupted
           ,@lockedToCharacter
           ,@note
           ,@frameType
           ,@x
           ,@y
           ,@inventoryId
           ,@secDescrText
           ,@descrText
           ,@artFilename
           ,@duplicated
           ,@maxStackSize
           ,@stackSize
           ,@talismanTier
           ,@support
           ,@prophecyDiffText
           ,@prophecyText
           ,@isRelic
           ,@Stash_id)";
            itemCommand = new SqlCommand(query, dbConnection);
            itemCommand.Parameters.AddWithValue("@itemID", stashItem.id);
            itemCommand.Parameters.AddWithValue("@verified", stashItem.verified);
            itemCommand.Parameters.AddWithValue("@w", stashItem.w);
            itemCommand.Parameters.AddWithValue("@h", stashItem.h);
            itemCommand.Parameters.AddWithValue("@ilvl", stashItem.ilvl);
            itemCommand.Parameters.AddWithValue("@icon", stashItem.icon);
            itemCommand.Parameters.AddWithValue("@league", stashItem.league);
            itemCommand.Parameters.AddWithValue("@name", stashItem.name);
            itemCommand.Parameters.AddWithValue("@typeLine", stashItem.typeLine);
            itemCommand.Parameters.AddWithValue("@identified", stashItem.identified);
            itemCommand.Parameters.AddWithValue("@corrupted", stashItem.corrupted);
            itemCommand.Parameters.AddWithValue("@lockedToCharacter", stashItem.lockedToCharacter);
            itemCommand.Parameters.AddWithValue("@note", stashItem.note ?? "");
            itemCommand.Parameters.AddWithValue("@frameType", stashItem.frameType);
            itemCommand.Parameters.AddWithValue("@x", stashItem.x);
            itemCommand.Parameters.AddWithValue("@y", stashItem.y);
            itemCommand.Parameters.AddWithValue("@inventoryId", stashItem.inventoryId ?? "");
            itemCommand.Parameters.AddWithValue("@secDescrText", stashItem.secDescrText ?? "");
            itemCommand.Parameters.AddWithValue("@descrText", stashItem.descrText ?? "");
            itemCommand.Parameters.AddWithValue("@artFilename", stashItem.artFilename ?? "");
            itemCommand.Parameters.AddWithValue("@duplicated", stashItem.duplicated);
            itemCommand.Parameters.AddWithValue("@maxStackSize", stashItem.maxStackSize);
            itemCommand.Parameters.AddWithValue("@stackSize", stashItem.stackSize);
            itemCommand.Parameters.AddWithValue("@talismanTier", stashItem.talismanTier);
            itemCommand.Parameters.AddWithValue("@support", stashItem.support);
            itemCommand.Parameters.AddWithValue("@prophecyDiffText", stashItem.prophecyDiffText ?? "");
            itemCommand.Parameters.AddWithValue("@prophecyText", stashItem.prophecyText ?? "");
            itemCommand.Parameters.AddWithValue("@isRelic", stashItem.isRelic);
            itemCommand.Parameters.AddWithValue("@Stash_id", s.id);
            int.TryParse(itemCommand.ExecuteScalar().ToString(), out iID);
        }
        private static void insertProperties(SqlConnection dbConnection, List<Properties> propList, int iID, bool additional)
        {
            string query = $@"INSERT INTO [dbo].[Properties]
           ([name]
           ,[displayMode]
           ,[type]
           ,[progress]
           ,[Item_id]
           ,[value0]
           ,[value1]
           ,[additonal])
     VALUES
           (@name
           ,@displayMode
           ,@type
           ,@progress
           ,@Item_id
           ,@value0
           ,@value1
           ,@additonal)";
            if (propList != null)
            {
                SqlCommand propertyCommand = new SqlCommand(query, dbConnection);
                string command = "";
                foreach (var item in propList)
                {
                    propertyCommand = new SqlCommand(query, dbConnection);
                    propertyCommand.Parameters.AddWithValue("@name", item.name);
                    propertyCommand.Parameters.AddWithValue("@displayMode", item.displayMode);
                    propertyCommand.Parameters.AddWithValue("@type", item.type);
                    propertyCommand.Parameters.AddWithValue("@progress", item.progress);
                    propertyCommand.Parameters.AddWithValue("@Item_id", iID);
                    propertyCommand.Parameters.AddWithValue("@additonal", additional);

                    if (item.values != null && item.values.Count > 0)
                    {
                        propertyCommand.Parameters.AddWithValue("@value0", item.values.Children().Children().First().ToObject<string>());
                        propertyCommand.Parameters.AddWithValue("@value1", item.values.Children().Children().Last().ToObject<int>());
                    }
                    else
                    {
                        propertyCommand.Parameters.AddWithValue("@value1", DBNull.Value);
                        propertyCommand.Parameters.AddWithValue("@value0", DBNull.Value);
                    }
                    command += propertyCommand.CommandText;
                }
                if (command == "") return;

                propertyCommand.CommandText = command;
                    propertyCommand.ExecuteNonQuery();
                
            }

        }
        private static void insertRequirements(SqlConnection dbConnection, Item stashItem, int iID)
        {
            string query = $@"INSERT INTO [dbo].[Requirements]
           ([name]
           ,[displayMode]
           ,[type]
           ,[progress]
           ,[Item_id]
           ,[value0]
           ,[value1])
     VALUES
           (@name
           ,@displayMode
           ,@type
           ,@progress
           ,@Item_id
           ,@value0
           ,@value1)";
            if (stashItem.requirements != null)
            {
                SqlCommand propertyCommand = new SqlCommand(query, dbConnection);
                string command = "";
                foreach (var item in stashItem.requirements)
                {
                    propertyCommand = new SqlCommand(query, dbConnection);
                    propertyCommand.Parameters.AddWithValue("@name", item.name);
                    propertyCommand.Parameters.AddWithValue("@displayMode", item.displayMode);
                    propertyCommand.Parameters.AddWithValue("@type", item.type);
                    propertyCommand.Parameters.AddWithValue("@progress", item.progress);
                    propertyCommand.Parameters.AddWithValue("@Item_id", iID);
                    if (item.values != null)
                    {
                        propertyCommand.Parameters.AddWithValue("@value0", item.values.Children().Children().First().ToObject<string>());
                        propertyCommand.Parameters.AddWithValue("@value1", item.values.Children().Children().Last().ToObject<int>());
                    }
                    else
                    {
                        propertyCommand.Parameters.AddWithValue("@value1", DBNull.Value);
                        propertyCommand.Parameters.AddWithValue("@value0", DBNull.Value);
                    }
                    command += propertyCommand.CommandText;
                }
                if (command == "") return;

                propertyCommand.CommandText = command;
                propertyCommand.ExecuteNonQuery();

            }

        }
        private static void insertStash(SqlConnection dbConnection, Stash s)
        {
            if ((s.accountName ?? "") == "") return;
            string query = @"INSERT INTO[dbo].[Stashes]
            ([id]
           ,[accountName]
           ,[lastCharacterName]
           ,[stash]
           ,[stashType]
           ,[isPublic])
     VALUES
           (@id
           ,@accountName
           ,@lastCharacterName
           ,@stash
           ,@stashType
           ,@isPublic)";
            SqlCommand stashCommand = new SqlCommand(query, dbConnection);
            stashCommand.Parameters.AddWithValue("@id", s.id);
            stashCommand.Parameters.AddWithValue("@accountName", s.accountName);
            stashCommand.Parameters.AddWithValue("@lastCharacterName", s.lastCharacterName ?? "");
            stashCommand.Parameters.AddWithValue("@stash", s.stash);
            stashCommand.Parameters.AddWithValue("@stashType", s.stashType);
            stashCommand.Parameters.AddWithValue("@isPublic", s.isPublic);
            stashCommand.ExecuteNonQuery();
            i++;
        }

    }
}
