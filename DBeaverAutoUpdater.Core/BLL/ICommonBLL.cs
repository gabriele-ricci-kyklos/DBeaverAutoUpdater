namespace DBeaverAutoUpdater.Core.BLL
{
    public interface ICommonBLL
    {
        string GetOrCreateAppDataFolder();
        string CreateTempAppDataFolder();
    }
}