using Octokit;
using Spectrum.Resonator.Infrastructure.Markers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services.Interfaces
{
    public interface ISpectrumInstallerService : IService
    {
        Task<List<Release>> DownloadReleaseList();
        Task DownloadPackage(string assetUrl, string targetPath);
        Task InstallPackage(string sourcePath);

        bool ValidateDistanceInstallationPath(string path);
        bool ValidateSpectrumPackage(string path);
        string GetRegisteredDistanceInstallationPath();
    }
}
