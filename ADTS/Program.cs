using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using LiteDB;
using static ADTS.Tools;
using static ADTS.ADTSData;

namespace ADTS
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        //private static ArrayList categorysList = new ArrayList();
        public static LiteDatabase db = new LiteDatabase(@AppDomain.CurrentDomain.BaseDirectory + "MyData.db");
        [STAThread]
        
        static void Main()
        {
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            MessageBox.Show("number of categories: "+Tools.GetAllCategories().Count.ToString());

            ResetSystem();
            UpdateSystem();
            /*if (Tools.GetAllCategories().Count != 0)
            {
                UpdateSystem();
            }*/
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ADTS());

            
            Console.ReadLine();
            
        }

        
        public static void UpdateSystem()
        {
            if (Tools.GetAllCategories().Count == 0) return;

            var dbe_files = Directory.GetFiles(Tools.DBEDir);
            List<Document> no_common_words = new();
            List<Document> no_common_notions = new();
            int similarity_level = int.Parse(File.ReadAllText(Tools.similarityLevel));
            var scores = new List<decimal>();
            var list = new List<string>();
            foreach (string path in dbe_files)
            {
                
                Document dc = new Document(path);
                var catlist = Tools.GetAllCategories();
                foreach (Category ct in Tools.GetAllCategories())
                {
                    var mutual_words_sum = Tools.GetSimilarNotions(ct.NWeight, dc.NWeight).Values.Sum();
                    decimal dc_sum = dc.NWeight.Values.Sum();
                    decimal relative_score = (mutual_words_sum / dc_sum)*100;
                    scores.Add(relative_score);
                    if (relative_score >= similarity_level)
                    {
                        ct.MergeDictnionaries(dc);
                        Tools.UpdateCategory(ct);
                        dc.catlist.Add(ct.Name);
                    }
                }
                if(dc.catlist.Count != 0)
                {
                    Tools.SaveDocument(dc);
                    File.Move(path, Tools.fileDir + dc.Name);
                }
                scores.Add(-1);
            }
            var a = "";
        }
        public static void ResetSystem()
        {
            foreach (string path in Directory.GetFiles(Tools.fileDir)){
                File.Move(path, Tools.DBEDir + Tools.GetDocName(path));
            }
            db.GetCollection<Category>("categories").DeleteAll();
            db.GetCollection<Document>("documents").DeleteAll();
        }

    }
}
