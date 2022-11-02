using System;
using System.Windows.Forms;

namespace FFXIAHScrape
{
    public static class Program
    {
        public static Form1 form1 = new Form1();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form1);
        }
    }
}