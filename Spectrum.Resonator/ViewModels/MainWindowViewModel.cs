using DevExpress.Mvvm;
using Spectrum.Resonator.Providers.Interfaces;

namespace Spectrum.Resonator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IStatusBarDataProvider _statusBarDataProvider;

        public MainWindowViewModel(IStatusBarDataProvider statusBarDataProvider)
        {
            _statusBarDataProvider = statusBarDataProvider;

            _statusBarDataProvider.SetActionInfo("Idle");
            _statusBarDataProvider.SetDetailedStatus("Waiting for user input...");
        }
    }
}
