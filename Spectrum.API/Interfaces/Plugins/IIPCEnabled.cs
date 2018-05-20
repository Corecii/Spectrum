using Spectrum.API.IPC;

namespace Spectrum.API.Interfaces.Plugins
{
    public interface IIPCEnabled
    {
        void HandleIPCData(IPCData data);
    }
}
