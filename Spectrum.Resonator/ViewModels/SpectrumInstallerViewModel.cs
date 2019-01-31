using DevExpress.Mvvm;
using Spectrum.Resonator.Models;
using Spectrum.Resonator.Services.Interfaces;

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

        public SpectrumInstallerViewModel(ISpectrumInstallerService spectrumInstallerService)
        {
            _spectrumInstallerService = spectrumInstallerService;
        }
    }
}
