using Microsoft.Win32;
using Octokit;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
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

        public async Task<string> DownloadPackage(string assetUrl)
        {
            var targetPath = Path.Combine(Path.GetTempPath(), "spectrum.zip");

            using (var webClient = new WebClient())
                await webClient.DownloadFileTaskAsync(assetUrl, targetPath);

            return targetPath;
        }

        public async Task<List<Release>> DownloadReleaseList()
        {
            return new List<Release>(await GitHubClient.Repository.Release.GetAll("Ciastex", "Spectrum"));
        }

        public async Task ExtractPackage(string sourcePath, string distancePath)
        {
            // Only call this when you've validated the package before.
            // Otherwise bad stuff will happen.

            using (var zipArchive = await Task.Run(() => ZipFile.OpenRead(sourcePath)))
                await Task.Run(() => zipArchive.ExtractToDirectory(Path.Combine(distancePath, "Distance_Data")));
        }

        public Task InstallSpectrum(string distancePath)
        {
            throw new System.NotImplementedException();
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
