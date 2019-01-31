using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        private readonly IStatusBarDataProvider _statusBarDataProvider;


        public DistanceInstallationInfo DistanceInstallationInfo { get; set; }
        public List<Release> AvailableReleases { get; set; }
        public Release PickedRelease { get; set; }

        public bool GetLatestRelease
        {
            get => GetProperty(() => GetLatestRelease);
            set => SetProperty(() => GetLatestRelease, value, () =>
            {
                if (value)
                    PickedRelease = AvailableReleases.OrderByDescending(x => x.CreatedAt).First();
            });
        }

        public SpectrumInstallerViewModel(ISpectrumInstallerService spectrumInstallerService, IStatusBarDataProvider statusBarDataProvider)
        {
            _spectrumInstallerService = spectrumInstallerService;
            _statusBarDataProvider = statusBarDataProvider;

            DistanceInstallationInfo = _spectrumInstallerService.GetDistanceInstallationStatus();
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
            var commonFileDialog = new CommonOpenFileDialog
            {
                Title = "Browse for Distance installation...",
                IsFolderPicker = true,
                Multiselect = false,
                ShowPlacesList = false,
                AllowNonFileSystemItems = false,
                EnsurePathExists = true
            };

            var result = commonFileDialog.ShowDialog(owner);

            if (result == CommonFileDialogResult.Ok)
            {
                DistanceInstallationInfo = new DistanceInstallationInfo
                {
                    InstallationPath = commonFileDialog.FileName,
                    IsInstalled = true
                };
            }
        }
    }
}
