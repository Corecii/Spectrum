using Microsoft.Win32;
using Octokit;
using Spectrum.Resonator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services
{
    public class SpectrumInstallerService : ISpectrumInstallerService
    {
        private GitHubClient GitHubClient { get; }

        public SpectrumInstallerService()
        {
            GitHubClient = new GitHubClient(new ProductHeaderValue("Spectrum.Resonator"));
        }

        public async Task DownloadPackage(string assetUrl, string targetPath)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Release>> DownloadReleaseList()
        {
            return new List<Release>(await GitHubClient.Repository.Release.GetAll("Ciastex", "Spectrum"));
        }

        public async Task InstallPackage(string sourcePath)
        {
            throw new NotImplementedException();
        }

        public bool ValidateDistanceInstallationPath(string path)
        {
            return File.Exists(Path.Combine(path, "Distance_Data", "Managed", "Assembly-CSharp.dll"))
                && File.Exists(Path.Combine(path, "Distance_Data", "Managed", "UnityEngine.dll"))
                && File.Exists(Path.Combine(path, "Distance.exe"));
        }

        public bool ValidateSpectrumPackage(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                ZipFile.OpenRead(path);
                return true;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }

        public string GetRegisteredDistanceInstallationPath()
        {
            var installationPath = string.Empty;

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var regKey = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 233610");

                if (regKey != null)
                {
                    installationPath = regKey.GetValue("InstallLocation", string.Empty) as string;
                    regKey.Dispose();

                }
            }

            return installationPath;
        }
    }
}
