using CustomNotes.Settings.UI;
using System;
using Zenject;

namespace CustomNotes.Managers
{
    class CustomNotesViewManager : IInitializable, IDisposable
    {
        private NoteListViewController _noteListViewController;
        private NoteModifierViewController _noteModifierViewController;
        private GameplaySetupViewController _gameplaySetupViewController;

        public CustomNotesViewManager(NoteListViewController noteListViewController, NoteModifierViewController noteModifierViewController, GameplaySetupViewController gameplaySetupViewController)
        {
            _noteListViewController = noteListViewController;
            _noteModifierViewController = noteModifierViewController;
            _gameplaySetupViewController = gameplaySetupViewController;
        }

        public void Initialize()
        {
            _noteListViewController.customNotesReloaded += NoteListViewController_CustomNotesReloaded;
            _gameplaySetupViewController.didActivateEvent += GameplaySetupViewController_DidActivateEvent;
            _gameplaySetupViewController.didDeactivateEvent += GameplaySetupViewController_DidDeactivateEvent; 
        }

        public void Dispose()
        {
            _noteListViewController.customNotesReloaded -= NoteListViewController_CustomNotesReloaded;
            _gameplaySetupViewController.didActivateEvent -= GameplaySetupViewController_DidActivateEvent;
            _gameplaySetupViewController.didDeactivateEvent -= GameplaySetupViewController_DidDeactivateEvent;
        }

        private void NoteListViewController_CustomNotesReloaded()
        {
            _noteModifierViewController.SetupList();
        }

        private void GameplaySetupViewController_DidActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _noteModifierViewController.ParentControllerActivated();
        }

        private void GameplaySetupViewController_DidDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _noteModifierViewController.ParentControllerDeactivated();
        }
    }
}
