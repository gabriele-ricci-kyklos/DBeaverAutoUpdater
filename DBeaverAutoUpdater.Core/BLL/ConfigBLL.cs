﻿using DBeaverAutoUpdater.Core.BE;
using GenericCore.Serialization.Xml;
using GenericCore.Support;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public class ConfigBLL : IConfigBLL
    {
        public string ConfigFilePath { get; private set; }

        public ConfigBLL()
        {
            string path = ConfigurationManager.AppSettings["ConfigurationFilePath"];
            if(path.IsNullOrBlankString())
            {
                throw new ArgumentException("No key 'ConfigurationFilePath' configured in the app config");
            }

            ConfigFilePath = path;
        }

        public ConfigurationItem RetrieveConfig()
        {
            CreateConfigFileIfNecessary();
            string xml = File.ReadAllText(ConfigFilePath);
            return QuickXmlSerializer.DeserializeObject<ConfigurationItem>(xml);
        }

        public void SaveConfiguration(ConfigurationItem configItem)
        {
            configItem.AssertNotNull(nameof(configItem));

            string xml = QuickXmlSerializer.SerializeObject(configItem);
            CreateConfigFileIfNecessary();
            File.WriteAllText(ConfigFilePath, xml);
        }

        public bool ConfigFileExists()
        {
            return File.Exists(ConfigFilePath);
        }

        private void CreateConfigFileIfNecessary()
        {
            if (!File.Exists(ConfigFilePath))
            {
                //TODO: default configuration here
                File.WriteAllText(ConfigFilePath, string.Empty);
            }
        }
    }
}