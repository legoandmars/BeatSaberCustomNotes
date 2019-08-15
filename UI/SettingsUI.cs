using HMUI;
using CustomUI.BeatSaber;
using CustomUI.MenuButton;
using CustomNotes.Utilities;

namespace CustomNotes.UI
{
    internal static class SettingsUI
    {
        internal static CustomMenu _notesMenu;

        internal static void CreateMenu()
        {
            if (_notesMenu == null)
            {
                _notesMenu = BeatSaberUI.CreateCustomMenu<CustomMenu>("Custom Notes");
                NoteListViewController noteListViewController = BeatSaberUI.CreateViewController<UI.NoteListViewController>();

                noteListViewController.backButtonPressed += delegate ()
                {
                    _notesMenu.Dismiss();
                };
                _notesMenu.SetMainViewController(noteListViewController, true);

                noteListViewController.DidSelectRowEvent += delegate (TableView view, int row)
                {
                    NoteAssetLoader.selectedNote = row;
                    Configuration.CurrentlySelectedNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote].FileName;
                };
            }

            MenuButtonUI.AddButton("CustomNotes", delegate () { _notesMenu.Present(); });
        }
    }
}
