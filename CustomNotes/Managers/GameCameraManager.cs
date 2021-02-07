using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    public class GameCameraManager : IInitializable, IDisposable
    {
        public Camera MainCamera { get; private set; } = null;
        public Camera[] Cameras { get; private set; }

        private PluginConfig _pluginConfig;

        [Inject]
        internal GameCameraManager(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(GameCameraManager)}!");
            Cameras = Camera.allCameras;
            MainCamera = Camera.main;
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
                foreach (Camera cam in Cameras)
                {
                    if (cam == MainCamera)
                    {
                        LayerUtils.SetCamera(cam, LayerUtils.CameraView.FirstPerson);
                    }
                    else
                    {
                        LayerUtils.SetCamera(cam, LayerUtils.CameraView.ThirdPerson);
                    }
                }
            }
        }

        public void Dispose()
        {
            Logger.log.Debug($"Disposing {nameof(GameCameraManager)}!");
            LayerUtils.DestroyWatermark();
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                foreach (Camera cam in Cameras)
                {
                    LayerUtils.SetCamera(cam, LayerUtils.CameraView.Default);
                }
            }
        }
    }
}
