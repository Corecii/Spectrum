using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Spectrum.Resonator.Services.Interfaces;
using System.Diagnostics;

namespace Spectrum.Resonator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ISpectrumInstallerService _spectrumInstallerService;

        public MainWindowViewModel(ISpectrumInstallerService spectrumInstallerService)
        {
            _spectrumInstallerService = spectrumInstallerService;
        }

        [Command]
        public async void Test()
        {
            var releases = await _spectrumInstallerService.GetReleaseList();

            foreach(var release in releases)
                Debug.WriteLine(release.Url);
        }
    }
}
