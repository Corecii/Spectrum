using Spectrum.Interop.Game.Network;

namespace Spectrum.Interop.Game.EventArgs.Network
{
    public class PlayerEventArgs : System.EventArgs
    {
        public string Nickname { get; private set; }
        public bool IsReady { get; private set; }
        public LevelCompatibility LevelCompatibility { get; private set; }

        public PlayerEventArgs(string nickname, bool isReady, LevelCompatibility levelCompatibility)
        {
            Nickname = nickname;
            IsReady = isReady;
            LevelCompatibility = levelCompatibility;
        }
    }
}
