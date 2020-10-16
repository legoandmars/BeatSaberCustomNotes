using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace CustomNotes.Settings.UI
{
    internal class SettingsUI
    {
        private static readonly MenuButton menuButton = new MenuButton("Custom Notes", "Change Custom Notes Here!", NotesMenuButtonPressed, true);

        public static NotesFlowCoordinator notesFlowCoordinator;
        public static bool created = false;

        public static void CreateMenu()
        {
            if (!created)
            {
                MenuButtons.instance.RegisterButton(menuButton);
                created = true;
            }
        }

        public static void RemoveMenu()
        {
            if (created)
            {
                MenuButtons.instance.UnregisterButton(menuButton);
                created = false;
            }
        }

        public static void ShowNotesFlow()
        {
            if (notesFlowCoordinator == null)
            {
                notesFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<NotesFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(notesFlowCoordinator, null);
        }

        private static void NotesMenuButtonPressed() => ShowNotesFlow();
    }
}
