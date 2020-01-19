using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace CustomNotes.Settings.UI
{
    internal class SettingsUI
    {
        public static NotesFlowCoordinator notesFlowCoordinator;
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
            if (notesFlowCoordinator == null)
            {
                notesFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<NotesFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(notesFlowCoordinator, null, false, false);
        }

        private static void NotesMenuButtonPressed() => ShowNotesFlow();
    }
}
