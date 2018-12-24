using GenericCore.Compression.Zip;
using GenericCore.Support;
using GenericCore.Support.Web;
using Microsoft.VisualBasic.FileIO;
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
    //TODO: improve FileSystem.CopyDirectory

    public class UpdateBLL : IUpdateBLL
    {
        public void UpdateVersion(string installationPath, byte[] zipArchive)
        {
            var files = Zipper.GetFileContentsFromZipFile(zipArchive);
            string backupPath = CreateTempFolder();

            SimpleLog.Info($"Creating backup of the current version at {backupPath}");
            BackupCurrentVersion(installationPath, backupPath);
            SimpleLog.Info($"Backup has been created");

            string tempPath = CreateTempFolder();
            foreach(var file in files)
            {
                File.WriteAllBytes(Path.Combine(tempPath, file.Item1), file.Item2);
            }

            IOUtilities.EmptyFolder(installationPath);
            FileSystem.CopyDirectory(tempPath, installationPath, true);

            IOUtilities.DeleteFolder(tempPath);
        }

        public void BackupCurrentVersion(string sourcePath, string destPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new ArgumentException($"The path {sourcePath} is either not a directory or does not exist");
            }

            FileSystem.CopyDirectory(sourcePath, destPath, true);
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

        public byte[] RetrieveZipFile(Architecture arch)
        {
            if(arch != Architecture.X86 || arch != Architecture.X64)
            {
                throw new ArgumentException($"Architecture {arch} not supported");
            }

            string url = GetDownloadUrl(arch);
            if(!WebDataRetriever.TryDownloadFile(url, out byte[] zipArchive))
            {
                throw new ArgumentException("An error occurred while downloading the file");
            }

            return zipArchive;
        }

        private string GetDownloadUrl(Architecture arch)
        {
            const string _baseDownloadUrl = "https://dbeaver.io/files/dbeaver-ce-latest-win32.win32.x86{0}.zip";

            if (arch != Architecture.X86 || arch != Architecture.X64)
            {
                throw new ArgumentException($"Architecture {arch} not supported");
            }

            return _baseDownloadUrl.FormatWith((arch == Architecture.X86) ? string.Empty : "_64");
        }
    }
}
