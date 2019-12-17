using HMUI;
using CustomNotes.Utilities;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage;
using BS_Utils.Utilities;
namespace CustomNotes.UI
{
    internal static class SettingsUI
    {
        internal static CustomNotesFlowCoordinator _notesFlowCoordinator;
        internal static bool created = false;
        internal static void CreateMenu()
        {
            if (created) return;
            MenuButtons.instance.RegisterButton(new MenuButton("Custom Notes", "Change Custom Notes Here!", NotesMenuButtonPressed, true));
            created = true;
        }

        internal static void ShowNotesFlow()
        {
            if (_notesFlowCoordinator == null)
                _notesFlowCoordinator = BeatSaberMarkupLanguage.BeatSaberUI.CreateFlowCoordinator<CustomNotesFlowCoordinator>();
           BeatSaberMarkupLanguage.BeatSaberUI.MainFlowCoordinator.InvokeMethod("PresentFlowCoordinator", _notesFlowCoordinator, null, false, false);
        }

        private static void NotesMenuButtonPressed()
        {
            //  Logger.logger.Info("Notes Menu Button Pressed");
            ShowNotesFlow();
        }
    }
}
