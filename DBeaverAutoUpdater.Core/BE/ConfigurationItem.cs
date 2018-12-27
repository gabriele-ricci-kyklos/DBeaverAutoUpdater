using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BE
{
    public class ConfigurationItem
    {
        public string DBeaverInstallPath { get; set; }
        public Architecture? Architecture { get; set; }
    }
}
