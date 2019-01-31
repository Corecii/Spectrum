using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Octokit;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Providers.Interfaces;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Spectrum.Resonator.ViewModels
{
    public class SpectrumInstallerViewModel : ViewModelBase
    {
        private readonly ISpectrumInstallerService _spectrumInstallerService;
        private readonly IStatusBarDataProvider _statusBarDataProvider;

        private DistanceInstallationInfo _distanceInstallationInfo;

        public DistanceInstallationInfo DistanceInstallationInfo
        {
            get
            {
                var steamVersionInstallationInfo = _spectrumInstallerService.GetDistanceInstallationStatus();
                _distanceInstallationInfo = steamVersionInstallationInfo ?? (_distanceInstallationInfo = new DistanceInstallationInfo());

                return _distanceInstallationInfo;
            }
        }

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

            DownloadAvailableReleases();
        }

        [Command]
        public async void DownloadAvailableReleases()
        {
            _statusBarDataProvider.SetActionInfo("Downloading...");
            _statusBarDataProvider.SetDetailedStatus("Getting release list");

            AvailableReleases = await _spectrumInstallerService.DownloadReleaseList();

            if (AvailableReleases.Count > 0)
                PickedRelease = AvailableReleases.First();

            _statusBarDataProvider.Reset();
        }
    }
}
