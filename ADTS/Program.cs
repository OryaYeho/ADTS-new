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
            int new_docs = 0;

            foreach (string path in dbe_files)
            {
                
                Document dc = new Document(path);
                var catlist = Tools.GetAllCategories();
                report += dc.Name + " ";
                foreach (Category ct in Tools.GetAllCategories())
                {
                    decimal relative_score = SimilarityLevel(ct, dc);
                    report += "\n"+ct.Name + " " + relative_score.ToString().Split(".")[0] +" similar notions: "+Tools.DictionaiesIntersection(ct.NWeight,dc.NWeight).Count;
                    
                    if (relative_score>= similarity_level)
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
                    new_docs++;
                }
                else
                {
                    File.Move(path, Tools.NonRelevant + dc.Name);
                }
                report += "\n\n";
                
            }
            MessageBox.Show("New documents: " + new_docs + "\n\nNon Relevant: " + (dbe_files.Count() - new_docs));
            //File.WriteAllText(Tools.projDir+"report.txt", report);
            //Process.Start("notepad.exe", Tools.projDir + "report.txt");
            
        }
        public static void ResetSystem()
        {
            foreach (string path in Directory.GetFiles(Tools.fileDir)){
                File.Move(path, Tools.DBEDir + Tools.GetDocName(path));
            }
            foreach (string path in Directory.GetFiles(Tools.NonRelevant))
            {
                File.Move(path, Tools.DBEDir + Tools.GetDocName(path));
            }
            db.GetCollection<Category>("categories").DeleteAll();
            db.GetCollection<Document>("documents").DeleteAll();
        }

    }
}
