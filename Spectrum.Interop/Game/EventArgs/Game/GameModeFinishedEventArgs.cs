namespace Spectrum.Interop.Game.EventArgs.Game
{
    public class GameModeFinishedEventArgs : System.EventArgs
    {
        public Interop.Game.Network.NetworkGroup NetworkGroup { get; }

        public GameModeFinishedEventArgs(Interop.Game.Network.NetworkGroup networkGroup)
        {
            NetworkGroup = networkGroup;
        }
    }
}
