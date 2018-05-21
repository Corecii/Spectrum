using Spectrum.API.Interfaces.Systems;

namespace Spectrum.API.Interfaces.Plugins
{
    public interface IPlugin
    {
        string IPCIdentifier { get; set; }

        void Initialize(IManager manager);
    }
}
