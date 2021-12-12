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
            UpdateSystem();
            if (Tools.GetAllCategories().Count != 0)
            {
                UpdateSystem();
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ADTS());


            Console.ReadLine();
        }

        
        public static void UpdateSystem()
        {
            var all_categories = Tools.GetAllCategories();
            var dbe_files = Directory.GetFiles(Tools.DBEDir);
            foreach(string path in dbe_files)
            {
                Document dc = new Document(path);
                foreach(Category ct in all_categories)
                {

                }
            }
            
         }

    }
}
