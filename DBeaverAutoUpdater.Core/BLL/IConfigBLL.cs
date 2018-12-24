using DBeaverAutoUpdater.Core.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public interface IConfigBLL
    {
        ConfigurationItem RetrieveConfig();
        void SaveConfiguration(ConfigurationItem configItem);
        bool ConfigFileExists();
    }
}
