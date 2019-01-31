using Microsoft.Win32;
using Octokit;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services
{
    public class SpectrumInstallerService : ISpectrumInstallerService
    {
        private GitHubClient GitHubClient { get; }

        public SpectrumInstallerService()
        {
            GitHubClient = new GitHubClient(new ProductHeaderValue("Ciastex"));
        }

        public bool IsSpectrumInstalled
        {
            get
            {
                var status = GetDistanceInstallationStatus();

                if (!status.IsInstalled)
                    return false;

                if (!Directory.Exists(status.InstallationPath))
                    return false;

                return Directory.Exists(Path.Combine(status.InstallationPath, "Distance_Data", "Spectrum"));
            }
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

        public DistanceInstallationInfo GetDistanceInstallationStatus()
        {
            var installationInfo = new DistanceInstallationInfo();

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var regKey = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 233610");

                if (regKey != null)
                {
                    var installationPath = regKey.GetValue("InstallLocation", string.Empty) as string;
                    if (!string.IsNullOrWhiteSpace(installationPath))
                    {
                        installationInfo.IsInstalled = true;
                        installationInfo.InstallationPath = installationPath;
                    }

                    regKey.Dispose();
                }
            }

            return installationInfo;
        }
    }
}
