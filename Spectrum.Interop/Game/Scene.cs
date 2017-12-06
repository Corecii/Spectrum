using System;
using Spectrum.Interop.Game.EventArgs.Scene;

namespace Spectrum.Interop.Game
{
    public class Scene
    {
        public static event EventHandler<SceneLoadedEventArgs> BeginLoading;
        public static event EventHandler<SceneLoadedEventArgs> Loaded;

        static Scene()
        {
            Events.Scene.LoadFinish.Subscribe(data =>
            {
                var eventArgs = new SceneLoadedEventArgs(data.sceneName);
                Loaded?.Invoke(null, eventArgs);
            });

            Events.Scene.StartLoad.Subscribe(data =>
            {
                var eventArgs = new SceneLoadedEventArgs(data.sceneName);
                BeginLoading?.Invoke(null, eventArgs);
            });
        }
    }
}
