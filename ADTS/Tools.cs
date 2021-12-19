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
            HybridSA.DigitalTextImage.MakeMSTTR(FullText, ref nDictionaryWords, ref wordD, ref posD, 5, ref sts, ref TextCompression);
            FullText = sts;
            
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
        public static Dictionary<string, decimal> GetSimilarNotions(Dictionary<string, decimal> category, Dictionary<string, decimal> document)
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
        
        
    }
}
