using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using System;
using Zenject;

namespace CustomNotes.Managers
{
    public class WatermarkManager : IInitializable, IDisposable
    {

        private PluginConfig _pluginConfig;

        [Inject]
        internal WatermarkManager(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(WatermarkManager)}!");
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
            }
        }

        public void Dispose()
        {
            Logger.log.Debug($"Disposing {nameof(WatermarkManager)}!");
            LayerUtils.DestroyWatermark();
        }
    }
}
