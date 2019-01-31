using Octokit;
using Spectrum.Resonator.Infrastructure.Markers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectrum.Resonator.Services.Interfaces
{
    public interface ISpectrumInstallerService : IService
    {
        bool IsSpectrumInstalled { get; }

        Task<List<Release>> GetReleaseList();
        Task DownloadPackage(string releaseName, string targetPath);
        Task InstallPackage(string sourcePath);
    }
}
