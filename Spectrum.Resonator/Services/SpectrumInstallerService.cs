using Microsoft.Win32;
using Octokit;
using Spectrum.Resonator.Enums;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<PrismTerminationReason> InstallSpectrum(string distancePath, string customPrismArguments = null)
        {
            var prismProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = customPrismArguments ?? "-t Assembly-CSharp.dll -s Spectrum.Bootstrap.dll",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.Combine(distancePath, "Distance_Data", "Managed"),
                    FileName = Path.Combine(distancePath, "Distance_Data", "Managed", "Spectrum.Prism.exe")
                }
            };

            await Task.Run(() =>
            {
                prismProcess.Start();
                prismProcess.WaitForExit();
            });

            return (PrismTerminationReason)prismProcess.ExitCode;
        }

        public async Task UninstallSpectrum(string distancePath, bool steamValidate)
        {
            // TODO: Don't hardcode paths, preferably use validator service to retrieve some of this.
            var spectrumPath = Path.Combine(distancePath, "Distance_Data", "Spectrum");
            var additionalPaths = new List<string>
            {
                Path.Combine(distancePath, "Distance_Data", "Managed", "Spectrum.Bootstrap.dll"),
                Path.Combine(distancePath, "Distance_Data", "Managed", "Spectrum.Prism.exe"),
                Path.Combine(distancePath, "Distance_Data", "Managed", "Mono.Cecil.dll"),
                Path.Combine(distancePath, "Distance_Data", "Managed", "install_linux.sh"),
                Path.Combine(distancePath, "Distance_Data", "Managed", "install_windows.bat")
            };

            if (Directory.Exists(spectrumPath))
                Directory.Delete(spectrumPath, true);

            foreach (var path in additionalPaths)
                if (File.Exists(path))
                    File.Delete(path);

            if (steamValidate)
            {
                await Task.Run(() =>
                {
                    Process.Start("steam://validate/233610").WaitForExit();
                });
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

        public void ExtractPackage(string sourcePath, string distancePath)
        {
            // Only call this when you've validated the package before.
            // Otherwise bad stuff will happen.

            using (var zipArchive = ZipFile.OpenRead(sourcePath))
                zipArchive.ExtractToDirectory(Path.Combine(distancePath, "Distance_Data"));
        }
    }
}
