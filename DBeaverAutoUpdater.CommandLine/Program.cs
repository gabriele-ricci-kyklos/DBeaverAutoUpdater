using DBeaverAutoUpdater.Core.BLL;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleLog.SetLogFile(writeText: true, logLevel: SimpleLog.Severity.Exception);

            try
            {

            }
            catch (Exception ex)
            {
                SimpleLog.Error("An error occurred");
                SimpleLog.Log(ex);
            }
            finally
            {
                SimpleLog.Flush();
            }
        }
    }
}
