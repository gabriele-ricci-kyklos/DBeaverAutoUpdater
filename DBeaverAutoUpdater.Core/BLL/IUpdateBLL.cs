using DBeaverAutoUpdater.Core.BE;

namespace DBeaverAutoUpdater.Core.BLL
{
    public interface IUpdateBLL
    {
        void UpdateVersion(ConfigurationItem configItem);
    }
}