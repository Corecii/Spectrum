using Spectrum.Resonator.Infrastructure.Markers.Interfaces;
using Spectrum.Resonator.Models;

namespace Spectrum.Resonator.Providers.Interfaces
{
    public interface IStatusBarDataProvider : IProvider
    {
        StatusBarData Data { get; }

        void SetActionInfo(string newActionInfo);
        void SetDetailedStatus(string newDetailedStatus);
    }
}
