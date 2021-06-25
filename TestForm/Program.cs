using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new DownLoad_APM());
            //Application.Run(new DownLoad_EAP());
            //Application.Run(new DownLoad_TAP());
            Application.Run(new DownLoad_AA());
        }
    }
}
