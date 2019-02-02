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
        private const string ArchiveValidatorName = "Spectrum Archive Validator";
        private const string DistanceValidatorName = "Distance Installation Validator";

        private readonly IBrowseDialogService _browseDialogService;
        private readonly ISpectrumInstallerService _spectrumInstallerService;
        private readonly IStatusBarDataProvider _statusBarDataProvider;
        private readonly IValidatorService _validatorService;

        public List<Release> AvailableReleases { get; set; }
        public Release PickedRelease { get; set; }
        public bool InstallingLocalPackage { get; set; }
        public string DistanceInstallationPath { get; set; }
        public string LocalPackagePath { get; set; }

        public bool GettingLatestRelease
        {
            get => GetProperty(() => GettingLatestRelease);
            set => SetProperty(() => GettingLatestRelease, value, () =>
            {
                if (value)
                    PickedRelease = AvailableReleases.OrderByDescending(x => x.CreatedAt).First();
            });
        }

        public bool IsInstallationPossible
        {
            get
            {
                var distanceIsInstalled = _validatorService.ValidateFileSystemPath(DistanceInstallationPath, DistanceValidatorName);

                if (InstallingLocalPackage)
                {
                    var packageIsValid = _validatorService.ValidateZipArchive(LocalPackagePath, ArchiveValidatorName);
                    return distanceIsInstalled && packageIsValid;
                }
                else return distanceIsInstalled;
            }
        }

        public SpectrumInstallerViewModel(IBrowseDialogService browseDialogService, 
                                          ISpectrumInstallerService spectrumInstallerService,
                                          IStatusBarDataProvider statusBarDataProvider,
                                          IValidatorService validatorService)
        {
            _browseDialogService = browseDialogService;
            _spectrumInstallerService = spectrumInstallerService;
            _statusBarDataProvider = statusBarDataProvider;
            _validatorService = validatorService;

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
            if (InstallingLocalPackage)
            {
                _statusBarDataProvider.SetActionInfo("Installing local package...");
                await _spectrumInstallerService.ExtractPackage(LocalPackagePath, DistanceInstallationPath);
            }
            else
            {
                _statusBarDataProvider.SetActionInfo("Downloading release package...");
                var packagePath = await _spectrumInstallerService.DownloadPackage(PickedRelease.Assets.First().BrowserDownloadUrl);

                if (_validatorService.ValidateZipArchive(packagePath, ArchiveValidatorName))
                {
                    _statusBarDataProvider.SetActionInfo("Installing release package...");
                    await _spectrumInstallerService.ExtractPackage(packagePath, DistanceInstallationPath);
                }
                else
                {
                    // MessageBox, error, yadda yadda
                }
            }

            _statusBarDataProvider.Reset();
        }
    }
}
