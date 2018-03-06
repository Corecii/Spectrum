using Spectrum.API.Experimental;

namespace Spectrum.API.Interfaces.Plugins
{
    public interface IRequiresAssetLoad
    {
        void LoadAssets(RuntimeAssetLoader assets);
    }
}
