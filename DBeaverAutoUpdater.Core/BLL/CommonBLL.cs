using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public class CommonBLL : ICommonBLL
    {
        public string GetOrCreateAppDataFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "DBeaverAutoUpdater");
            Directory.CreateDirectory(specificFolder);
            return specificFolder;
        }

        public string CreateTempAppDataFolder()
        {
            string appDataFolder = GetOrCreateAppDataFolder();
            string tempFolder = Path.Combine(appDataFolder, DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }
    }
}
