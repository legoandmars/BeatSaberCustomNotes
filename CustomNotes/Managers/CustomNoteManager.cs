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

        internal CustomNoteManager([InjectOptional] Submission submission, NoteAssetLoader noteAssetLoader, GameplayCoreSceneSetupData gameplayCoreSceneSetupData, PluginConfig pluginConfig, IDifficultyBeatmap level)
        {
            _submission = submission;
            _noteAssetLoader = noteAssetLoader;
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
                    _submission?.DisableScoreSubmission("Custom Notes", "Ghost Notes");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows)
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Disappearing Arrows");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.smallCubes)
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Small Notes");
                }
                if (Utils.IsNoodleMap(_level))
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Noodle Extensions");
                }
            }
        }
    }
}