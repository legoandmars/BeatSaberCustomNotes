using Zenject;
using CustomNotes.Data;
using SiraUtil.Services;
using CustomNotes.Utilities;

namespace CustomNotes.Managers
{
    internal class CustomNoteManager : IInitializable
    {
        private readonly Submission _submission;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        internal CustomNoteManager(Submission submission, NoteAssetLoader noteAssetLoader, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _submission = submission;
            _noteAssetLoader = noteAssetLoader;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public void Initialize()
        {
            if (_noteAssetLoader.SelectedNote != 0)
            {
                CustomNote activeNote = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                MaterialSwapper.GetMaterials();
                MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteRight);
                if (_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes)
                {
                    _submission.DisableScoreSubmission("Custom Notes", "Ghost Notes");
                }
                if (_gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows)
                {
                    _submission.DisableScoreSubmission("Custom Notes", "Disappearing Arrows");
                }
            }
        }
    }
}