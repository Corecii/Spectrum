using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Octokit;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Providers.Interfaces;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Spectrum.Resonator.ViewModels
{
    public class SpectrumInstallerViewModel : ViewModelBase
    {
        private readonly ISpectrumInstallerService _spectrumInstallerService;
        private readonly IBrowseDialogService _browseDialogService;
        private readonly IStatusBarDataProvider _statusBarDataProvider;

        public List<Release> AvailableReleases { get; set; }
        public Release PickedRelease { get; set; }
        public bool InstallLocalPackage { get; set; }
        public string DistanceInstallationPath { get; set; }
        public string LocalPackagePath { get; set; }

        public bool GetLatestRelease
        {
            get => GetProperty(() => GetLatestRelease);
            set => SetProperty(() => GetLatestRelease, value, () =>
            {
                if (value)
                    PickedRelease = AvailableReleases.OrderByDescending(x => x.CreatedAt).First();
            });
        }

        public bool IsInstallationPossible
        {
            get
            {
                var distanceIsInstalled = _spectrumInstallerService.ValidateDistanceInstallationPath(DistanceInstallationPath);
                var packageIsValid = false;

                if (InstallLocalPackage)
                    packageIsValid = _spectrumInstallerService.ValidateSpectrumPackage(LocalPackagePath);

                return distanceIsInstalled && packageIsValid;
            }
        }

        public SpectrumInstallerViewModel(ISpectrumInstallerService spectrumInstallerService,
                                          IBrowseDialogService browseDialogService,
                                          IStatusBarDataProvider statusBarDataProvider)
        {
            _spectrumInstallerService = spectrumInstallerService;
            _browseDialogService = browseDialogService;
            _statusBarDataProvider = statusBarDataProvider;

            DistanceInstallationPath = _spectrumInstallerService.GetRegisteredDistanceInstallationPath();

            DownloadAvailableReleases();
        }

        [Command]
        public async void DownloadAvailableReleases()
        {
            _statusBarDataProvider.SetActionInfo("Downloading release list...");

            AvailableReleases = await _spectrumInstallerService.DownloadReleaseList();

            if (AvailableReleases.Count > 0)
                PickedRelease = AvailableReleases.First();

            _statusBarDataProvider.Reset();
        }

        [Command]
        public void BrowseForDistanceInstallation(Window owner)
        {
            var path = _browseDialogService.Browse(new BrowseDialogSettings
            {
                Owner = owner,
                IsFolderPicker = true,
                Title = "Browse for Distance installation directory..."
            });

            if (!string.IsNullOrWhiteSpace(path))
                DistanceInstallationPath = path;
        }

        [Command]
        public void BrowseForLocalPackage(Window owner)
        {
            var path = _browseDialogService.Browse(new BrowseDialogSettings
            {
                Owner = owner,
                IsFolderPicker = false,
                Title = "Browse for local Spectrum package..."
            });

            if (!string.IsNullOrWhiteSpace(path))
                LocalPackagePath = path;
        }

        [Command]
        public async void BeginInstallation(Window owner)
        {
            if (InstallLocalPackage)
            {
                _statusBarDataProvider.SetActionInfo("Installing local package...");
                await _spectrumInstallerService.InstallPackage(LocalPackagePath);
            }
            else
            {
                // download, unpack, install
            }

            _statusBarDataProvider.Reset();
        }
    }
}
