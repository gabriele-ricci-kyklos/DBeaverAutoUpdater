using DBeaverAutoUpdater.Core.BE;
using DBeaverAutoUpdater.Core.Support.Logging;
using GenericCore.Compression.Zip;
using GenericCore.Support;
using GenericCore.Support.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public class UpdateBLL : IUpdateBLL
    {
        public ICommonBLL CommonBLL => new CommonBLL();

        public async Task UpdateVersion(ConfigurationItem configItem, IProgress<double> progress = null)
        {
            configItem.AssertNotNull(nameof(configItem));

            string appDataOldVersionBackupPath = CommonBLL.CreateTempAppDataFolder();
            string appDataPath = CommonBLL.CreateTempAppDataFolder();
            string appDataUnzippedPath = Path.Combine(appDataPath, "unzipped");

            try
            {
                Task<byte[]> zipArchiveTask = RetrieveZipFileWithBackup(configItem.Architecture, appDataPath, progress);

                Task backupTask =
                    Task.Run(() =>
                    {
                        Logger.Info($"Creating a backup of the current version at {appDataOldVersionBackupPath}");
                        BackupCurrentVersion(configItem.DBeaverInstallPath, appDataOldVersionBackupPath);
                        Logger.Info($"The backup has been created");
                    });

                await Task.WhenAll(zipArchiveTask, backupTask);

                Logger.Info("Unzipping the downloaded archive");
                byte[] zipArchive = zipArchiveTask.Result;
                UnzipArchive(zipArchive, appDataUnzippedPath, progress);
                Logger.Info("The archive has been successfully unzipped");

                Logger.Info($"Patching DBeaver current version at {configItem.DBeaverInstallPath}");
                IOUtilities.EmptyFolder(configItem.DBeaverInstallPath);
                IOUtilities.CopyFolderTo(Path.Combine(appDataUnzippedPath, "dbeaver"), configItem.DBeaverInstallPath, true);
                Logger.Info("Patching successful");
            }
            finally
            {
                Logger.Info("Deleting temp data");
                IOUtilities.DeleteFolder(appDataPath);
                IOUtilities.DeleteFolder(appDataOldVersionBackupPath);
                Logger.Info("Temp data deleted successfully");
            }
        }

        public void UnzipArchive(byte[] zipArchive, string rootPath, IProgress<double> progress = null)
        {
            zipArchive.AssertNotNull(nameof(zipArchive));
            rootPath.AssertHasText(nameof(rootPath));

            IList<Tuple<string, byte[]>> files = Zipper.GetFileContentsFromZipFile(zipArchive);
            double singleFileReportValue = 1 / (double)files.Count * 100;
            object lockObj = new object();
            double reportSumValue = 0;

            Parallel
                .ForEach
                (
                    files,
                    file =>
                    {
                        string filePath = Path.Combine(rootPath, file.Item1);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        File.WriteAllBytes(filePath, file.Item2);

                        if (progress.IsNotNull())
                        {
                            lock (lockObj)
                            {
                                reportSumValue += singleFileReportValue;
                                progress.Report(reportSumValue);
                            }
                        }
                    }
                );

            progress.Report(100);
        }

        private void BackupCurrentVersion(string sourcePath, string destPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new ArgumentException($"The path {sourcePath} is either not a directory or does not exist");
            }

            IOUtilities.CopyFolderTo(sourcePath, destPath, true);
        }

        private async Task<byte[]> RetrieveZipFileWithBackup(Architecture arch, string backupPath, IProgress<double> progress = null)
        {
            backupPath.AssertHasText(nameof(backupPath));

            if (arch != Architecture.X86 && arch != Architecture.X64)
            {
                throw new ArgumentException($"Architecture {arch} not supported");
            }

            string url = GetDownloadUrl(arch);
            string fileName = Utilities.GetFileNameFromUriString(url);

            Logger.Info("Downloading the zip archive");

            byte[] zipArchive = null;
            try
            {
                zipArchive = await WebDataRetriever.DownloadFileAsync(url, progress);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred while downloading the file", ex);
            }
            finally
            {
                Logger.Info("Finished downloading the zip archive");
            }

            string localPath = Path.Combine(backupPath, fileName);
            Logger.Info($"Writing the zip archive locally to {backupPath}");
            File.WriteAllBytes(localPath, zipArchive);
            Logger.Info("The zip archive has been successfully saved locally");

            return zipArchive;
        }

        private string GetDownloadUrl(Architecture arch)
        {
            const string _baseDownloadUrl = "https://dbeaver.io/files/dbeaver-ce-latest-win32.win32.x86{0}.zip";

            if (arch != Architecture.X86 && arch != Architecture.X64)
            {
                throw new ArgumentException($"Architecture {arch} not supported");
            }

            return _baseDownloadUrl.FormatWith((arch == Architecture.X86) ? string.Empty : "_64");
        }
    }
}
