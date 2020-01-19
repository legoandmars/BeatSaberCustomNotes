using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils.Utilities;

namespace CustomNotes.Settings.UI
{
    internal class SettingsUI
    {
        public static NotesFlowCoordinator _notesFlowCoordinator;
        public static bool created = false;

        public static void CreateMenu()
        {
            if (!created)
            {
                MenuButton menuButton = new MenuButton("Custom Notes", "Change Custom Notes Here!", NotesMenuButtonPressed, true);
                MenuButtons.instance.RegisterButton(menuButton);

                created = true;
            }
        }

        public static void ShowNotesFlow()
        {
            if (_notesFlowCoordinator == null)
            {
                _notesFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<NotesFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.InvokeMethod("PresentFlowCoordinator", _notesFlowCoordinator, null, false, false);
        }

        private static void NotesMenuButtonPressed() => ShowNotesFlow();
    }
}
