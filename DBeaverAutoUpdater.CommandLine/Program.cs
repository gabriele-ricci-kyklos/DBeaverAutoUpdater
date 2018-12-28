using DBeaverAutoUpdater.Core.BE;
using DBeaverAutoUpdater.Core.BLL;
using GenericCore.Support;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.CommandLine
{
    class Program
    {
        const string _argCreateConfigFile = "-c";

        static void Main(string[] args)
        {
            SimpleLog.SetLogFile(writeText: true, logLevel: SimpleLog.Severity.Exception);
            IConfigBLL configBLL = new ConfigBLL();
            IUpdateBLL updateBLL = new UpdateBLL();

            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == _argCreateConfigFile)
                    {
                        if (args.Length < 3)
                        {
                            Console.Error.WriteLine($"Wrong usage of {_argCreateConfigFile}");
                        }

                        configBLL
                            .SaveConfiguration
                            (
                                new ConfigurationItem
                                {
                                    DBeaverInstallPath = args[1].TrimStart().TrimEnd(),
                                    Architecture = args[2].ToEnum(Architecture.X86)
                                }
                            );

                        return;
                    }
                }

                if (!configBLL.ConfigFileExists())
                {
                    Console.WriteLine($"Configuration file not found, please call with line arguments {_argCreateConfigFile} DBeaverInstallPath Architecture");
                    return;
                }

                ConfigurationItem configItem = configBLL.RetrieveConfiguration();
                updateBLL.UpdateVersion(configItem);
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
