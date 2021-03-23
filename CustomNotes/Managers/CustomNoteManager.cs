using Zenject;
using CustomNotes.Data;
using SiraUtil.Services;
using CustomNotes.Utilities;
using System;
using CustomNotes.Settings.Utilities;
using IPA.Loader;
using System.Linq;

namespace CustomNotes.Managers
{
    internal class CustomNoteManager : IInitializable
    {
        private readonly Submission _submission;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;
        private readonly PluginConfig _pluginConfig;
        private readonly IDifficultyBeatmap _level;

        public Flags _customNoteFlags { get; private set; }

        internal CustomNoteManager([InjectOptional] Submission submission, NoteAssetLoader noteAssetLoader, Flags customNoteFlags, GameplayCoreSceneSetupData gameplayCoreSceneSetupData, PluginConfig pluginConfig, IDifficultyBeatmap level)
        {
            _submission = submission;
            _noteAssetLoader = noteAssetLoader;
            _customNoteFlags = customNoteFlags;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
            _pluginConfig = pluginConfig;
            _level = level;
        }
        public void Initialize()
        {
            if (_noteAssetLoader.SelectedNote != 0)
            {
                CustomNote activeNote = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];

                if (activeNote.NoteBomb != null)
                {
                    MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteBomb);
                }

                if (_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes)
                {
                    if (_pluginConfig.DisableOnModifier) _customNoteFlags.GhostNotesEnabled = true;
                    else _submission?.DisableScoreSubmission("Custom Notes", "Ghost Notes");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows)
                {
                    if (_pluginConfig.DisableOnModifier) _customNoteFlags.ForceDisable = true;
                    else _submission?.DisableScoreSubmission("Custom Notes", "Disappearing Arrows");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.smallCubes)
                {
                    if (_pluginConfig.DisableOnModifier) _customNoteFlags.ForceDisable = true;
                    else _submission?.DisableScoreSubmission("Custom Notes", "Small Notes");
                }
                if (Utils.IsNoodleMap(_level))
                {
                    if (_pluginConfig.DisableOnNoodle)
                    {
                        _customNoteFlags.ForceDisable = true;
                    }
                    else
                    {
                        _submission?.DisableScoreSubmission("Custom Notes", "Noodle Extensions");
                    }
                }
            }
        }

        public class Flags
        {
            public event Action onAnyFlagUpdate;

            private bool _forceDisable = false;
            public bool ForceDisable {
                get => _forceDisable;
                set
                {
                    _forceDisable = value;
                    onAnyFlagUpdate?.Invoke();
                }
            }

            private bool _ghostNotesEnabled = false;
            public bool GhostNotesEnabled {
                get => _ghostNotesEnabled;
                set
                {
                    _ghostNotesEnabled = value;
                    onAnyFlagUpdate?.Invoke();
                }
            }

            private int _firstFrameCount = 0;
            public int FirstFrameCount {
                get => _firstFrameCount;
                set
                {
                    _firstFrameCount = value;
                    onAnyFlagUpdate?.Invoke();
                }
            }
        }
    }
}