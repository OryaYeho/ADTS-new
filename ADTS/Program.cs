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
using System.Globalization;
using System.Diagnostics;

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
            var a = Tools.GetAllDocuments();
            var b = Tools.GetAllCategories();
            //ResetSystem();
            UpdateSystem();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ADTS());
            Application.Exit();
        }

        
        public static void UpdateSystem()
        {
            if (Tools.GetAllCategories().Count == 0) return;

            var dbe_files = Directory.GetFiles(Tools.DBEDir);
            int similarity_level = int.Parse(File.ReadAllText(Tools.similarityLevel));
            string report = "";

            foreach (string path in dbe_files)
            {
                
                Document dc = new Document(path);
                var catlist = Tools.GetAllCategories();
                report += dc.Name + " ";
                foreach (Category ct in Tools.GetAllCategories())
                {
                    /*var number_of_common_notions = Tools.GetSimilarNotions(ct.NWeight, dc.NWeight).Count;
                    var mutual_words_sum = Tools.GetSimilarNotions(ct.NWeight, dc.NWeight).Values.Sum();
                    decimal dc_sum = dc.NWeight.Values.Sum();
                    decimal relative_score = (mutual_words_sum / dc_sum)*100;*/

                    decimal relative_score = SimilarityLevel(ct, dc);
                    report += "\n"+ct.Name + " " + relative_score.ToString().Split(".")[0] +" similar notions: "+Tools.DictionaiesIntersection(ct.NWeight,dc.NWeight).Count;
                    
                    if (false)
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
                report += "\n\n";
                
            }
            File.WriteAllText(Tools.projDir+"report.txt", report);
            Process.Start("notepad.exe", Tools.projDir + "report.txt");
            
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
