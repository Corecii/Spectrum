using Octokit;
using Spectrum.Resonator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool IsSpectrumInstalled => true;

        public async Task DownloadPackage(string releaseName, string targetPath)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Release>> GetReleaseList()
        {
            return new List<Release>(await GitHubClient.Repository.Release.GetAll("Ciastex", "Spectrum"));
        }

        public async Task InstallPackage(string sourcePath)
        {
            throw new NotImplementedException();
        }
    }
}
