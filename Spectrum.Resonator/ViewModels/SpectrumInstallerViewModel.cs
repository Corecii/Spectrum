using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Octokit;
using Spectrum.Resonator.Enums;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Providers.Interfaces;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
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
        public bool IsSteamRelease { get; set; }
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

            if (!string.IsNullOrWhiteSpace(DistanceInstallationPath))
                IsSteamRelease = true;

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
                _spectrumInstallerService.ExtractPackage(LocalPackagePath, DistanceInstallationPath);
            }
            else
            {
                _statusBarDataProvider.SetActionInfo("Downloading release package...");
                var packagePath = await _spectrumInstallerService.DownloadPackage(PickedRelease.Assets.First().BrowserDownloadUrl);

                if (_validatorService.ValidateZipArchive(packagePath, ArchiveValidatorName))
                {
                    _statusBarDataProvider.SetActionInfo("Extracting release package...");
                    try
                    {
                        _spectrumInstallerService.ExtractPackage(packagePath, DistanceInstallationPath);
                    }
                    catch (IOException)
                    {
                        var dialogResult = MessageBox.Show(owner, "Error", "Spectrum appears to have been already installed. Reinstall?", MessageBoxButton.YesNo);

                        if (dialogResult == MessageBoxResult.Yes)
                            await _spectrumInstallerService.UninstallSpectrum(DistanceInstallationPath, IsSteamRelease);
                        else
                        {
                            _statusBarDataProvider.Reset();
                            return;
                        }

                        _spectrumInstallerService.ExtractPackage(packagePath, DistanceInstallationPath);
                    }

                    var result = await _spectrumInstallerService.InstallSpectrum(DistanceInstallationPath);
                    if (result != PrismTerminationReason.Default)
                    {
                        MessageBox.Show(owner, "Installation failed.\nExit code: {(int)result}.\nReason: {result}.");
                    }
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
