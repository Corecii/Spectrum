using Spectrum.Interop.Game.Vehicle;

namespace Spectrum.Interop.Game.EventArgs.Vehicle
{
    public class DestroyedEventArgs : System.EventArgs
    {
        public DestructionCause Cause { get; private set; }

        public DestroyedEventArgs(DestructionCause cause)
        {
            Cause = cause;
        }
    }
}
