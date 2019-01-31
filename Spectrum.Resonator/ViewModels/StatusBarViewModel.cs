using DevExpress.Mvvm;
using Spectrum.Resonator.Providers.Interfaces;

namespace Spectrum.Resonator.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        private readonly IStatusBarDataProvider _statusBarDataProvider;

        public string ActionInfo => _statusBarDataProvider.Data.ActionInfo;
        public string DetailedStatus => _statusBarDataProvider.Data.DetailedStatus;

        public StatusBarViewModel(IStatusBarDataProvider statusBarDataProvider)
        {
            _statusBarDataProvider = statusBarDataProvider;
        }
    }
}
