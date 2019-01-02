using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Spectrum.API.Logging;

namespace Spectrum.API.Storage
{
    public class FileSystem
    {
        public string RootDirectory { get; }
        public string DirectoryPath => Path.Combine(RootDirectory, Defaults.PrivateDataDirectory);

        private static Logger Log { get; set; }

        static FileSystem()
        {
            Log = new Logger(Defaults.FileSystemLogFileName);
        }

        public FileSystem()
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
        }

        public bool FileExists(string path)
        {
            var targetFilePath = Path.Combine(DirectoryPath, path);
            return File.Exists(targetFilePath);
        }

        public bool DirectoryExists(string path)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, path);
            return Directory.Exists(targetDirectoryPath);
        }

        public bool PathExists(string path)
        {
            return FileExists(path) || DirectoryExists(path);
        }

        public string CreateFile(string fileName, bool overwrite = false)
        {
            var targetFilePath = Path.Combine(DirectoryPath, fileName);

            if (File.Exists(targetFilePath))
            {
                if (overwrite)
                {
                    RemoveFile(fileName);
                }
                else
                {
                    Log.Error($"Couldn't create a PluginData file for path '{targetFilePath}'. The file already exists.");
                    return string.Empty;
                }
            }

            try
            {
                File.Create(targetFilePath).Dispose();
                return targetFilePath;
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't create a PluginData file for path '{targetFilePath}'.");
                Log.Exception(ex);
                return string.Empty;
            }
        }

        public void RemoveFile(string fileName)
        {
            var targetFilePath = Path.Combine(DirectoryPath, fileName);

            if (!File.Exists(targetFilePath))
            {
                Log.Error($"Couldn't delete a PluginData file for path '{targetFilePath}'. File does not exist.");
                return;
            }

            try
            {
                File.Delete(targetFilePath);
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't delete a PluginData file for path '{targetFilePath}'.");
                Log.Exception(ex);
            }
        }

        public void IterateOver(string directoryPath, Action<string> action)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, directoryPath);

            if (!Directory.Exists(targetDirectoryPath))
            {
                Log.Error($"Cannot iterate over directory at '{targetDirectoryPath}'. It doesn't exist.");
                return;
            }

            var paths = Directory.GetFiles(targetDirectoryPath);
            foreach (var path in paths)
            {
                try
                {
                    action(path);
                }
                catch (Exception e)
                {
                    Log.Error("Action for the file '{path}' failed. See file system exception log for details.");
                    Log.ExceptionSilent(e);

                    return;
                }
            }
        }

        public List<string> GetDirectories(string directoryPath)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, directoryPath);

            if (!Directory.Exists(targetDirectoryPath))
            {
                Log.Error($"Cannot get files in directory at '{targetDirectoryPath}'. It doesn't exist.");
                return null;
            }

            return Directory.GetDirectories(targetDirectoryPath).ToList();
        }

        public List<string> GetFiles(string directoryPath)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, directoryPath);

            if (!Directory.Exists(targetDirectoryPath))
            {
                Log.Error($"Cannot get files in directory at '{targetDirectoryPath}'. It doesn't exist.");
                return null;
            }

            return Directory.GetFiles(targetDirectoryPath).ToList();
        }

        public FileStream OpenFile(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            var targetFilePath = Path.Combine(DirectoryPath, fileName);

            if (!File.Exists(targetFilePath))
            {
                Log.Error($"Couldn't open the file. The requested file: '{targetFilePath}' does not exist.");
                return null;
            }

            try
            {
                return File.Open(targetFilePath, fileMode, fileAccess, fileShare);
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't open a PluginData file for path '{targetFilePath}'.");
                Log.Exception(ex);

                return null;
            }
        }

        public FileStream OpenFile(string fileName)
        {
            return OpenFile(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        }

        public string CreateDirectory(string directoryName)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, directoryName);

            try
            {
                Directory.CreateDirectory(targetDirectoryPath);
                return targetDirectoryPath;
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't create a PluginData directory for path '{targetDirectoryPath}'.");
                Log.Exception(ex);
                return string.Empty;
            }
        }

        public void RemoveDirectory(string directoryName)
        {
            var targetDirectoryPath = Path.Combine(DirectoryPath, directoryName);

            if (!Directory.Exists(targetDirectoryPath))
            {
                Log.Error($"Couldn't remove a PluginData directory for path '{targetDirectoryPath}'. Directory does not exist.");
                return;
            }

            try
            {
                Directory.Delete(targetDirectoryPath, true);
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't remove a PluginData directory for path '{targetDirectoryPath}'.");
                Log.Exception(ex);
            }
        }

        public static string GetValidFileName(string dirtyFileName, string replaceInvalidCharsWith = "_")
        {
            return Resource.GetValidFileName(dirtyFileName, replaceInvalidCharsWith);
        }

        public static string GetValidFileNameToLower(string dirtyFileName, string replaceInvalidCharsWith = "_")
        {
            return Resource.GetValidFileNameToLower(dirtyFileName, replaceInvalidCharsWith);
        }
    }
}
