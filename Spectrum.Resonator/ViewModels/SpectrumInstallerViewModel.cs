using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Octokit;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Spectrum.Resonator.ViewModels
{
    public class SpectrumInstallerViewModel : ViewModelBase
    {
        private readonly ISpectrumInstallerService _spectrumInstallerService;
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

        public List<Release> AvailableReleases
        {
            get => GetProperty(() => AvailableReleases);
            set => SetProperty(() => AvailableReleases, value);
        }

        public Release PickedRelease
        {
            get => GetProperty(() => PickedRelease);
            set => SetProperty(() => PickedRelease, value);
        }

        public SpectrumInstallerViewModel(ISpectrumInstallerService spectrumInstallerService)
        {
            _spectrumInstallerService = spectrumInstallerService;
        }

        [Command]
        public async void DownloadAvailableReleases()
        {
            AvailableReleases = await _spectrumInstallerService.DownloadReleaseList();

            if (AvailableReleases.Count > 0)
                PickedRelease = AvailableReleases.First();
        }
    }
}
