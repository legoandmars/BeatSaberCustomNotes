using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using System;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    public class GameCameraManager : IInitializable, IDisposable
    {
        public Camera MainCamera { get; }

        private PluginConfig _pluginConfig;

        [Inject]
        internal GameCameraManager(PluginConfig pluginConfig, Camera mainCamera)
        {
            _pluginConfig = pluginConfig;
            MainCamera = mainCamera;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(GameCameraManager)}!");
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.FirstPerson);
            }
        }

        public void Dispose()
        {
            Logger.log.Debug($"Disposing {nameof(GameCameraManager)}!");
            LayerUtils.DestroyWatermark();
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.Default);
            }
        }
    }
}
