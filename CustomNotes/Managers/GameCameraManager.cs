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

        private LayerUtils.CameraView _cameraView = LayerUtils.CameraView.FirstPerson;

        [Inject]
        internal GameCameraManager(PluginConfig pluginConfig, CustomNoteManager.Flags customNoteFlags)
        {
            _pluginConfig = pluginConfig;
            _customNoteFlags = customNoteFlags;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(GameCameraManager)}!");
            _customNoteFlags.OnAnyFlagUpdate += _customNoteFlags_OnAnyFlagUpdate;

            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
            }

            // Wait a tiny amount of time for the multiplayer camera to do it's thing
            SharedCoroutineStarter.instance.StartCoroutine(Utils.DoAfter(0.1f, () => {
                MainCamera = Camera.main;
                if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
                {
                    LayerUtils.SetCamera(MainCamera, _cameraView);
                }
            }));
        }

        private void _customNoteFlags_OnAnyFlagUpdate()
        {
            if(_customNoteFlags.ForceDisable || _customNoteFlags.GhostNotesEnabled)
            {
                SetActive(false);
                return;
            }

            SetActive(true);
        }

        private void SetActive(bool active)
        {
            LayerUtils.SetWatermarkActive(active);

            if (active)
            {
                _cameraView = LayerUtils.CameraView.FirstPerson;
            }
            else
            {
                _cameraView = LayerUtils.CameraView.Default;
            }

            LayerUtils.SetCamera(MainCamera, _cameraView);
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
