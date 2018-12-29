using DBeaverAutoUpdater.Core.BE;
using GenericCore.Serialization.Xml;
using GenericCore.Support;
using System;
using System.IO;

namespace DBeaverAutoUpdater.Core.BLL
{
    public class ConfigBLL : IConfigBLL
    {
        public ICommonBLL CommonBLL => new CommonBLL();
        public string ConfigFilePath => Path.Combine(CommonBLL.GetOrCreateAppDataFolder(), "config.xml");

        public ConfigurationItem RetrieveConfiguration()
        {
            try
            {
                CreateConfigFileIfNecessary();
                string xml = File.ReadAllText(ConfigFilePath);
                ConfigurationItem item = QuickXmlSerializer.DeserializeObject<ConfigurationItem>(xml);

                if (!CheckConfiguration(item))
                {
                    throw new ArgumentException("Some errors occurred in retrieving the configuration");
                }

                return item;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Some errors occurred in retrieving the configuration", ex);
            }
        }

        private bool CheckConfiguration(ConfigurationItem item)
        {
            if(item.IsNull())
            {
                return false;
            }

            if(item.DBeaverInstallPath.IsNullOrBlankString())
            {
                return false;
            }

            return true;
        }

        public void SaveConfiguration(ConfigurationItem configItem)
        {
            configItem.AssertNotNull(nameof(configItem));

            CreateConfigFileIfNecessary();
            string xml = QuickXmlSerializer.SerializeObject(configItem);
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
                using (Stream s = File.Create(ConfigFilePath));
            }
        }
    }
}
