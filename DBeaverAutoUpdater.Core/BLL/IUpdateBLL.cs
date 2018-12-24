using System.Runtime.InteropServices;

namespace DBeaverAutoUpdater.Core.BLL
{
    public interface IUpdateBLL
    {
        byte[] RetrieveZipFile(Architecture arch);
        void UpdateVersion(string installationPath, byte[] zipArchive);
    }
}