using DBeaverAutoUpdater.Core.BE;
using DBeaverAutoUpdater.Core.Support.Logging;
using GenericCore.Compression.Zip;
using GenericCore.Support;
using GenericCore.Support.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.BLL
{
    public class UpdateBLL : IUpdateBLL
    {
        public void UpdateVersion(ConfigurationItem configItem)
        {
            configItem.AssertNotNull(nameof(configItem));

            string backupPath = CreateTempFolder();
            string localPath = CreateTempFolder();

            try
            {
                Task<byte[]> zipArchiveTask = Task.Factory.StartNew(() => RetrieveZipFileWithBackup(configItem.Architecture, localPath));

                Logger.Info($"Creating a backup of the current version at {backupPath}");
                BackupCurrentVersion(configItem.DBeaverInstallPath, backupPath);
                Logger.Info($"The backup has been created");

                zipArchiveTask.Wait();
                byte[] zipArchive = zipArchiveTask.Result;
                UnzipArchiveWithBackup(zipArchive, localPath);
            }
            catch(Exception)
            {
                IOUtilities.DeleteFolder(localPath);
                throw;
            }

            Logger.Info($"Patching DBeaver current version at {configItem.DBeaverInstallPath}");
            IOUtilities.EmptyFolder(configItem.DBeaverInstallPath);
            IOUtilities.CopyFolderTo(localPath, configItem.DBeaverInstallPath, true);
            Logger.Info("Patching successful");

            IOUtilities.DeleteFolder(backupPath);
            IOUtilities.DeleteFolder(localPath);
        }

        private void UnzipArchiveWithBackup(byte[] zipArchive, string rootPath)
        {
            zipArchive.AssertNotNull(nameof(zipArchive));
            rootPath.AssertHasText(nameof(rootPath));

            Logger.Info("Unzipping the downloaded archive");

            IList<Tuple<string, byte[]>> files = Zipper.GetFileContentsFromZipFile(zipArchive);
            Parallel
                .ForEach
                (
                    files,
                    file =>
                    {
                        string filePath = Path.Combine(rootPath, file.Item1);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        File.WriteAllBytes(filePath, file.Item2);
                    }
                );

            Logger.Info("The archive has been successfully zipped");
        }

        private void BackupCurrentVersion(string sourcePath, string destPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new ArgumentException($"The path {sourcePath} is either not a directory or does not exist");
            }

            IOUtilities.CopyFolderTo(sourcePath, destPath, true);
        }

        private string CreateTempFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "DBeaverAutoUpdater");
            string backupFolder = Path.Combine(specificFolder, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            Directory.CreateDirectory(specificFolder);
            Directory.CreateDirectory(backupFolder);
            return backupFolder;
        }

        private byte[] RetrieveZipFileWithBackup(Architecture arch, string backupPath)
        {
            backupPath.AssertHasText(nameof(backupPath));

            if (arch != Architecture.X86 && arch != Architecture.X64)
            {
                throw new ArgumentException($"Architecture {arch} not supported");
            }

            string url = GetDownloadUrl(arch);
            string fileName = Utilities.GetFileNameFromUriString(url);

            Logger.Info("Downloading the zip archive");
            if(!WebDataRetriever.TryDownloadFile(url, out byte[] zipArchive))
            {
                throw new ArgumentException("An error occurred while downloading the file");
            }
            Logger.Info("Finished downloading the zip archive");

            string localPath = Path.Combine(backupPath, fileName);
            Logger.Info($"Writing the zip archive locally to {localPath}");
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
