using Spectrum.API.Interfaces.Systems;

namespace Spectrum.API.Interfaces.Plugins
{
    public interface IPlugin
    {
        void Initialize(IManager manager);
        void Shutdown();
    }
}
