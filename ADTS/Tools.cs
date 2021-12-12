using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using LiteDB;

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
        public static readonly string similarityLevel = projDir + "Settings\\Similarity Level.txt";

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
            TransformTextToNotions.Transformation(FullText, nDictionaryWords, wordD, posD, ref NotionAR, ref SWlist);
        }
        public static void prntArr(object[] arr)
        {
            foreach (object obj in arr)
            {
                Console.WriteLine(obj.ToString() + '\n');
            }
        }
        public class Document
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string path { get; set; }
            //public ArrayList SWList { get; set; } = new();
            //public ArrayList NList { get; set; } = new();
            public List<string> catlist { get; set; } = new();

            public Dictionary<string, decimal> NWeight { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> SWWeight { get; set; } = new Dictionary<string, decimal>();
            public Document(string path, string category){
                this.path = path;
                string[] splt = path.Split('\\');
                this.Name = splt[splt.Length - 1];
                string[] sw = new string[] { };
                string[] notions = new string[] { };
                Tools.DLLTransformation(path, ref notions, ref sw);
                
                this.catlist.Add(category);
                for (int i = 0; i < notions.Length; i++)
                {
                    string notionLen = notions[i].Split(')')[0];
                    string notion = notions[i].Split(')')[1].Split('=')[0];
                    string temp = notions[i].Split(')')[1].Split('=')[1];
                    decimal weight = decimal.Parse(notions[i].Split(')')[1].Split('=')[1]);

                    //NList.Add(notion);
                    NWeight.TryAdd(notion, weight);

                }
                for (int i = 1; i < sw.Length; i++)
                {
                    string word = sw[i].Split('=')[0];
                    int weight = Int32.Parse(sw[i].Split('=')[1]);

                    //SWList.Add(word);
                    SWWeight.TryAdd(word, weight);
                    
                }

            }
        }
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Dictionary<string, decimal> NWeight { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> SWWeight { get; set; } = new Dictionary<string, decimal>();

            //public List<Document> arr { get; set; }
            
            public Category(string name, List<Document> arr)
            {
                //this.arr = arr;
                this.Name = name;
                foreach(Document dc in arr)
                {
                    MergeDictnionaries(dc);
                }
            }
            [BsonCtor]
            public Category(int _id,string Name, Dictionary<string, decimal> NWeight, Dictionary<string, decimal> SWWeight)
            {
                this.Id = _id;
                this.Name = Name;
                this.NWeight = NWeight;
                this.SWWeight = SWWeight;
            }
            public Category() { }
            public void MergeDictnionaries(Document dc)
            {
                foreach (var entry in dc.NWeight)
                {
                    NWeight.TryAdd(entry.Key, entry.Value);
                }

                foreach (var entry in dc.SWWeight)
                {
                    SWWeight.TryAdd(entry.Key, entry.Value);
                }
            }
        }

        public static void SaveCategory(Category ct)
        {
            //var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Category>("categories");
            col.Insert(ct);
            Program.db.Commit();
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
        public static Document GetDocument(string dcname)
        {
           // var db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
            var col = Program.db.GetCollection<Document>("documents");
            var dclist = col.Query().Where(x => x.Name.Equals(dcname)).ToList();
            Program.db.Commit();
            return dclist.Count == 0 ? null : dclist[0];
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

        public static string GetDocName(string path)
        {
            string[] splt = path.Split('\\');
            return splt[splt.Length - 1];
        }
    }
}
