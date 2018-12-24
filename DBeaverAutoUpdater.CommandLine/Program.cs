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
        }
    }
}
