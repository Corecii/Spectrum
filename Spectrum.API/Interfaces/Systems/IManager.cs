using Spectrum.API.Experimental;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IManager
    {
        IHotkeyManager Hotkeys { get; }
        RuntimeAssetLoader Assets { get; }
    }
}
