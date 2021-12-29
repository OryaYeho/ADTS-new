using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using LiteDB;
using static ADTS.ADTSData;

namespace ADTS
{
    class Tools
    {
        /*path for the project - equals to: ...\Source\Repos\ConsoleApp2\ConsoleApp2\bin\Debug\
         * this path is like resources folder*/
        public static readonly string projDir = AppDomain.CurrentDomain.BaseDirectory;
        //path for the dictionary
        public static readonly string dicDir = projDir + "GF Wordley DictionaryE.txt";
        //path for the categories folder
        public static readonly string fileDir = projDir + "DBI\\";
        //path for the DBE folder
        public static readonly string DBEDir = projDir + "DBE\\";
        //Non Relevant folder
        public static readonly string NonRelevant = projDir+"Non Relevant\\";
        //settings folder
        public static readonly string settings = projDir + "Settings\\";
        //similairty level
        public static readonly string similarityLevel = projDir + "SimilarityLevel.txt";

        //documents to category map
        public static readonly string categoriesMap = settings + "Categories Map.txt";
        


        public static void DLLTransformation(string txtPath,ref string [] NotionAR, ref string [] SWlist)
        {
            //string FullText = File.ReadAllText(txtPath); //***relevant to dll***
            string FullText = File.ReadAllText(txtPath);
            string dictionary = File.ReadAllText(dicDir);
            string[] lines = dictionary.Split('\n');
            int nDictionaryWords = lines.Length; //***relevant to dll***
            var wordDList = new List<string>();
            var posDList = new List<string>();
            foreach (string line in lines)
            {
                string[] words = line.Split(',');
                wordDList.Add(words[0]);
                posDList.Add(words[1]);
            }
            string[] wordD = wordDList.ToArray(); //***relevant to dll***
            string[] posD = posDList.ToArray(); //***relevant to dll***
            string TextCompression = "";
            string sts = "";
            //HybridSA.DigitalTextImage.MakeMSTTR(FullText, ref nDictionaryWords, ref wordD, ref posD, 5, ref sts, ref TextCompression);
            //FullText = sts;
            
            TransformTextToNotions.Transformation(FullText, nDictionaryWords, wordD, posD, ref NotionAR, ref SWlist);
        }
        public static void prntArr(object[] arr)
        {
            foreach (object obj in arr)
            {
                Console.WriteLine(obj.ToString() + '\n');
            }
        }
        

        //
        //****Database Tools****
        //

        public static void SaveCategory(Category ct)
        {
            Program.db.BeginTrans();
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Category>("categories");
            col.Insert(ct);
            
            Program.db.Commit();
            return;
        }
        public static void SaveDocument(Document dc)
        {
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            col.Insert(dc);
            Program.db.Commit();
        }
        public static Category  GetCategory(string catname)
        {
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Category>("categories");
            var catlist = col.Query().Where(x => x.Name.Equals(catname)).ToList();
            Program.db.Commit();
            return catlist.Count == 0 ? null : catlist[0];
        }
        public static Document GetDocumentByName(string dcname)
        {
           // var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            var dclist = col.Query().Where(x => x.Name.Equals(dcname)).ToList();
            Program.db.Commit();
            return dclist.Count == 0 ? null : dclist[0];
        }
        public static List<Document> GetDocumentByCatName(string catname)
        {
            var col = Program.db.GetCollection<Document>("documents");
            return col.Query().Where(x => x.catlist.Contains(catname)).ToList();
        }
            public static List<Category> GetAllCategories()
        {
            
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Category>("categories");
            //Program.db.Commit();
            return col.Query().ToList();
        }
        public static List<Document> GetAllDocuments()
        {

            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            //Program.db.Commit();
            return col.Query().ToList();
        }

        public static void UpdateDocument(Document dc)
        {
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            col.Update(dc);
            Program.db.Commit();
        }
        public static void UpdateCategory(Category ct)
        {
            var col = Program.db.GetCollection<Category>("categories");
            col.Update(ct);
            Program.db.Commit();
        }

        public static void TempAdd(Document dc)
        {
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            col.Delete(col.Insert(dc));
            Program.db.Commit();
        }







        //
        //*******function tools*******
        //
        public static string GetDocName(string path)
        {
            string[] splt = path.Split('\\');
            return splt[splt.Length - 1];
        }

        //returns dictionary of mutual words
        public static Dictionary<string, decimal> DictionaiesIntersection(Dictionary<string, decimal> category, Dictionary<string, decimal> document)
        {
            Dictionary<string, decimal> result = new();
            foreach(var pair in document)
            {
                if (category.ContainsKey(pair.Key))
                {
                    result.Add(pair.Key,pair.Value);
                }
            }
            return result;
        }
        public static Dictionary<string, decimal> DictionaiesDisjoint(Dictionary<string, decimal> dic1, Dictionary<string, decimal> dic2)
        {
            Dictionary<string, decimal> result = new();
            foreach (var pair in dic2)
            {
                if (!dic1.ContainsKey(pair.Key))
                {
                    result.Add(pair.Key, dic2[pair.Key]);
                }
            }
            return result;
        }
        public static Dictionary<string, decimal> SplitNotionsToWord(Dictionary<string, decimal> NotionesDictionary)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            foreach(var pair in NotionesDictionary)
            {
                foreach(string word in pair.Key.Split("->"))
                {
                    result.TryAdd(word, 0);
                }
            }
            return result;
        }

        public static decimal SimilarityLevel(Category cat, Document dc)
        {
            decimal similarity = 0;
            decimal NotionsWeight = 0, Q = 4, WordsWeight = 0;
            var SharedWords = DictionaiesIntersection(cat.SWWeight, dc.SWWeight);
            /*var SharedNotions = DictionaiesIntersection(cat.NWeight, dc.NWeight);
            foreach (var pair in SharedNotions)
            {
                foreach(string word in pair.Key.Split("->"))
                {
                    NotionsWeight += cat.SWWeight[word];
                }
            }

            var WordsNotInNotions = DictionaiesDisjoint(dc.SWWeight, SplitNotionsToWord(SharedNotions));*/

            foreach(var pair in SharedWords)
            {
                similarity += cat.SWWeight[pair.Key];
            }
            foreach(var pair in dc.SWWeight)
            {
                WordsWeight += cat.SWWeight.ContainsKey(pair.Key) ? cat.SWWeight[pair.Key] : pair.Value;
            }
            return (similarity / WordsWeight) * 100;
            




            /*foreach (string notion in dc.NWeight.Keys)
            {
                bool NotionContainsWord = false;
                bool WordHasHighScore = false;
                foreach(string word in notion.Split("->"))
                {
                    if (cat.SWWeight.ContainsKey(word))
                    {
                        NotionContainsWord = true;


                    }
                }
                if (NotionContainsWord)
                {
                    score += dc.NWeight[notion];
                }
             return (score/dc.NWeight.Values.Sum())*100;
            }*/


        }


    }
}
