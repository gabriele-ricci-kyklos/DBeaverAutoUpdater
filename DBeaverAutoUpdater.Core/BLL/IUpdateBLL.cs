using DBeaverAutoUpdater.Core.BE;
using System;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public interface IUpdateBLL
    {
        Task UpdateVersion(ConfigurationItem configItem, IProgress<double> progress = null);
        void UnzipArchive(byte[] zipArchive, string rootPath, IProgress<double> progress = null);
    }
}