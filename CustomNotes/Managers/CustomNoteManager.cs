using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Submissions;
using Zenject;

namespace CustomNotes.Managers
{
    internal class CustomNoteManager : IInitializable
    {
        private readonly Submission _submission;
        private readonly PluginConfig _pluginConfig;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameCameraManager _gameCameraManager;
        private readonly IDifficultyBeatmap _difficultyBeatmap;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        internal CustomNoteManager([InjectOptional] Submission submission, PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, GameCameraManager gameCameraManager, IDifficultyBeatmap difficultyBeatmap, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _submission = submission;
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _gameCameraManager = gameCameraManager;
            _difficultyBeatmap = difficultyBeatmap;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
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

                if (_pluginConfig.AutoDisable && (_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes || _gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows ||
                    _gameplayCoreSceneSetupData.gameplayModifiers.smallCubes || Utils.IsNoodleMap(_difficultyBeatmap)))
                {
                    return;
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
                if (Utils.IsNoodleMap(_difficultyBeatmap))
                {
                    _submission?.DisableScoreSubmission("Custom Notes", "Noodle Extensions");
                }
            }
        }
    }
}