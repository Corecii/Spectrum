using System;
using Spectrum.API.Input;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IHotkeyManager
    {
        void Bind(Hotkey hotkey, Action action);
        void Bind(string hotkey, Action action);
        void Bind(string hotkeyString, Action action, bool isOneTime);

        void Unbind(string hotkeyString);
        void UnbindAll();
    }
}
