namespace Spectrum.Interop.Game.EventArgs.Scene
{
    public class SceneLoadedEventArgs : System.EventArgs
    {
        public string SceneName { get; }

        public SceneLoadedEventArgs(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
