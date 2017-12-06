using Spectrum.Interop.TypeWrappers;

namespace Spectrum.Interop.Game.EventArgs.Vehicle
{
    public class RespawnEventArgs : System.EventArgs
    {
        public Position Position { get; private set; }
        public Rotation Rotation { get; private set; }
        public bool WasFastRespawn { get; private set; }

        public RespawnEventArgs(Position position, Rotation rotation, bool wasFastRespawn)
        {
            Position = position;
            Rotation = rotation;
            WasFastRespawn = wasFastRespawn;
        }
    }
}
