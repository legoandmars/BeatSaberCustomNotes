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

        private PluginConfig _pluginConfig;
        private CustomNoteManager.Flags _customNoteFlags;
        private MainCamera _mainCamera;

        [Inject]
        internal GameCameraManager(PluginConfig pluginConfig, CustomNoteManager.Flags customNoteFlags, MainCamera mainCamera)
        {
            _pluginConfig = pluginConfig;
            _customNoteFlags = customNoteFlags;
            _mainCamera = mainCamera;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(GameCameraManager)}!");
            _customNoteFlags.OnAnyFlagUpdate += _customNoteFlags_OnAnyFlagUpdate;

            // Awake() hasn't been called yet so _mainCamera.camera is still null
            MainCamera = _mainCamera.GetComponent<Camera>();

            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.FirstPerson);
            }
        }

        private void _customNoteFlags_OnAnyFlagUpdate()
        {
            if(_customNoteFlags.ForceDisable || _customNoteFlags.GhostNotesEnabled)
            {
                SetWatermarkAndLayerActive(false);
                return;
            }

            SetWatermarkAndLayerActive(true);
        }

        private void SetWatermarkAndLayerActive(bool active)
        {
            LayerUtils.SetWatermarkActive(active);

            if (active)
            {
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.FirstPerson);
                return;
            }

            LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.Default);
        }

        public void Dispose()
        {
            Logger.log.Debug($"Disposing {nameof(GameCameraManager)}!");
            LayerUtils.DestroyWatermark();
            _customNoteFlags.OnAnyFlagUpdate -= _customNoteFlags_OnAnyFlagUpdate;
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.Default);
            }
        }
    }
}
