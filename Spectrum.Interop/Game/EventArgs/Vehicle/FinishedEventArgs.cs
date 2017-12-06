using Spectrum.Interop.Game.Vehicle;

namespace Spectrum.Interop.Game.EventArgs.Vehicle
{
    public class FinishedEventArgs : System.EventArgs
    {
        public RaceEndType Type { get; private set; }
        public int FinalTime { get; private set; }

        public FinishedEventArgs(RaceEndType type, int finalTime)
        {
            Type = type;
            FinalTime = finalTime;
        }
    }
}
