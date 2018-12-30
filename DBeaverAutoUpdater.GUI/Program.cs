using DBeaverAutoUpdater.Core.BLL;
using DBeaverAutoUpdater.Core.Support.Logging;
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
            Logger.Initialize(mode: LoggingMode.Both, useBackgroundTask: true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IConfigBLL configBLL = new ConfigBLL();

            if (!configBLL.ConfigFileExists())
            {
                Application.Run(new ConfigForm());
            }
            else
            {

            }
        }
    }
}
