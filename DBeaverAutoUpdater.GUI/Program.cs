using DBeaverAutoUpdater.Core.BLL;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBeaverAutoUpdater.GUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            SimpleLog.SetLogFile(writeText: true, logLevel: SimpleLog.Severity.Exception);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            IConfigBLL bll = new ConfigBLL();
            bll.RetrieveConfiguration();
        }
    }
}
