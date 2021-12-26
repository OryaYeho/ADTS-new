using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;

namespace ADTS
{
    class ADTSData
    {
        public class Document
        {
            public int Id { get; set; }
            public string Name { get; set; }
           // public string path { get; set; }
            //public ArrayList SWList { get; set; } = new();
            //public ArrayList NList { get; set; } = new();
            public List<string> catlist { get; set; } = new();

            public Dictionary<string, decimal> NWeight { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> SWWeight { get; set; } = new Dictionary<string, decimal>();

            public Document(string path, string category)
                : this(path)
            {

                this.catlist.Add(category);
            }

            public Document(string path)
            {
                //this.path = path;
                string[] splt = path.Split('\\');
                this.Name = splt[splt.Length - 1];
                string[] sw = new string[] { };
                string[] notions = new string[] { };
                Tools.DLLTransformation(path, ref notions, ref sw);
                
                //this.catlist.Add(category);
                for (int i = 0; i < notions.Length; i++)
                {
                    //string notionLen = notions[i].Split(')')[0];
                    string notion = notions[i].Split('=')[0];
                    //string temp = notions[i].Split(')')[1].Split('=')[1];
                    decimal weight = decimal.Parse(notions[i].Split('=')[1], NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);

                    //NList.Add(notion);
                    NWeight.TryAdd(notion, weight);

                }
                string FullText = File.ReadAllText(path).ToLower();
                for (int i = 1; i < sw.Length; i++)
                {
                    string word = sw[i].Split('=')[0];
                    int weight = Int32.Parse(sw[i].Split('=')[1], NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);

                    //SWList.Add(word);
                    SWWeight.TryAdd(word, Regex.Matches(FullText,word).Count);
                }

            }
            public Document() { }
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
                foreach (Document dc in arr)
                {
                    MergeDictnionaries(dc);
                }
            }
            [BsonCtor]
            public Category(int _id, string Name, Dictionary<string, decimal> NWeight, Dictionary<string, decimal> SWWeight)
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
                    if (SWWeight.ContainsKey(entry.Key))
                    {
                        SWWeight[entry.Key] += entry.Value;
                    }
                    else
                    {
                        SWWeight.Add(entry.Key, entry.Value);
                    }
                    
                }
            }
        }
    }
}
