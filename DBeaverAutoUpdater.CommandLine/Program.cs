﻿using DBeaverAutoUpdater.Core.BE;
using DBeaverAutoUpdater.Core.BLL;
using DBeaverAutoUpdater.Core.Support.Logging;
using GenericCore.Support;
using System;
using System.Runtime.InteropServices;

namespace DBeaverAutoUpdater.CommandLine
{
    class Program
    {
        const string _argCreateConfigFile = "-c";

        //static void Main(string[] args)
        //{
        //    Logger.Initialize(mode: LoggingMode.Both, useBackgroundTask: true);
        //    Logger.Info("lol");
        //    Logger.Flush();
        //}

        static void Main(string[] args)
        {
            Logger.Initialize(mode: LoggingMode.Both, useBackgroundTask: true);

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
                                    Architecture = args[2].ToUpperInvariant().ToEnum(Architecture.X86)
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
                updateBLL.UpdateVersion(configItem).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred", ex);
            }
            finally
            {
                Logger.Flush();
            }
        }
    }
}
