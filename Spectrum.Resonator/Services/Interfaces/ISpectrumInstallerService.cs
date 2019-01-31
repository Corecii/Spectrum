using Octokit;
using Spectrum.Resonator.Infrastructure.Markers.Interfaces;
using Spectrum.Resonator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services.Interfaces
{
    public interface ISpectrumInstallerService : IService
    {
        bool IsSpectrumInstalled { get; }

        DistanceInstallationInfo GetDistanceInstallationStatus();

        Task<List<Release>> DownloadReleaseList();
        Task DownloadPackage(string assetUrl, string targetPath);
        Task InstallPackage(string sourcePath);
    }
}
