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

        [Inject]
        internal GameCameraManager(PluginConfig pluginConfig, CustomNoteManager.Flags customNoteFlags)
        {
            _pluginConfig = pluginConfig;
            _customNoteFlags = customNoteFlags;
        }

        public void Initialize()
        {
            Logger.log.Debug($"Initializing {nameof(GameCameraManager)}!");
            _customNoteFlags.onAnyFlagUpdate += _customNoteFlags_onAnyFlagUpdate;
            MainCamera = Camera.main;
            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                LayerUtils.CreateWatermark();
                LayerUtils.SetCamera(MainCamera, LayerUtils.CameraView.FirstPerson);
            }
        }

        private void _customNoteFlags_onAnyFlagUpdate()
        {
            if(_customNoteFlags.ForceDisable || _customNoteFlags.GhostNotesEnabled)
            {
                _customNoteFlags.onAnyFlagUpdate -= _customNoteFlags_onAnyFlagUpdate;
                Dispose();
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
