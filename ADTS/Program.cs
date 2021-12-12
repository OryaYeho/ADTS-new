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
            
            var a = Tools.GetAllCategories();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ADTS());


            Console.ReadLine();
        }
        


    }
}
