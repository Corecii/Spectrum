using Octokit;
using Spectrum.Resonator.Infrastructure.Markers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services.Interfaces
{
    public interface ISpectrumInstallerService : IService
    {
        Task<List<Release>> DownloadReleaseList();
        Task<string> DownloadPackage(string assetUrl);
        Task ExtractPackage(string sourcePath, string distancePath);
        Task InstallSpectrum(string distancePath);

        string GetRegisteredDistanceInstallationPath();
    }
}
