using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.MenuButtons;
using CustomNotes.Settings.UI;
using System;
using Zenject;

namespace CustomNotes.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly NotesFlowCoordinator _notesFlowCoordinator;

        private NoteQuickAccessController _noteQuickAccessController;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, NotesFlowCoordinator notesFlowCoordinator, NoteQuickAccessController noteQuickAccessController)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _notesFlowCoordinator = notesFlowCoordinator;
            _menuButton = new MenuButton("Custom Notes", "Change Custom Notes Here!", ShowNotesFlow, true);
            _noteQuickAccessController = noteQuickAccessController;
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
            GameplaySetup.instance.AddTab("Custom Notes", "CustomNotes.Settings.UI.Views.quickAccess.bsml", _noteQuickAccessController);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
            if(GameplaySetup.IsSingletonAvailable)
            {
                GameplaySetup.instance.RemoveTab("Custom Notes");
            }
        }

        private void ShowNotesFlow()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_notesFlowCoordinator);
        }
    }
}