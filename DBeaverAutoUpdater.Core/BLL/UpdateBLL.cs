using DBeaverAutoUpdater.Core.BE;
using GenericCore.Compression.Zip;
using GenericCore.Support;
using GenericCore.Support.Web;
using SimpleLogger;
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

                SimpleLog.Info($"Creating a backup of the current version at {backupPath}");
                BackupCurrentVersion(configItem.DBeaverInstallPath, backupPath);
                SimpleLog.Info($"The backup has been created");

                zipArchiveTask.Wait();
                byte[] zipArchive = zipArchiveTask.Result;

                SimpleLog.Info("Unzipping the downloaded archive");
                IList<Tuple<string, byte[]>> files = Zipper.GetFileContentsFromZipFile(zipArchive);
                Parallel.ForEach(files, file => File.WriteAllBytes(Path.Combine(localPath, file.Item1), file.Item2));
                SimpleLog.Info("The archive has been successfully zipped");
            }
            finally
            {
                IOUtilities.DeleteFolder(localPath);
            }

            SimpleLog.Info($"Patching DBeaver current version at {configItem.DBeaverInstallPath}");
            IOUtilities.EmptyFolder(configItem.DBeaverInstallPath);
            IOUtilities.CopyFolderTo(localPath, configItem.DBeaverInstallPath, true);
            SimpleLog.Info("Patching successful");

            IOUtilities.DeleteFolder(backupPath);
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

            SimpleLog.Info("Downloading the zip archive");
            if(!WebDataRetriever.TryDownloadFile(url, out byte[] zipArchive))
            {
                throw new ArgumentException("An error occurred while downloading the file");
            }
            SimpleLog.Info("Finished downloading the zip archive");

            string localPath = Path.Combine(backupPath, fileName);
            SimpleLog.Info($"Writing the zip archive locally to {localPath}");
            File.WriteAllBytes(localPath, zipArchive);
            SimpleLog.Info("The zip archive has been successfully saved locally");

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
