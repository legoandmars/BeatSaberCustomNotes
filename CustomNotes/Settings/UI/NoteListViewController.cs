using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using CustomNotes.Data;
using CustomNotes.Utilities;
using HMUI;
using System.Linq;

namespace CustomNotes.Settings.UI
{
    internal class NotesListView : BeatSaberMarkupLanguage.ViewControllers.BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteList.bsml";

        [UIComponent("noteList")]
        public CustomListTableData customListTableData;

        [UIAction("noteSelect")]
        public void Select(TableView tableView, int row)
        {
            NoteAssetLoader.SelectedNote = row;
            Configuration.CurrentlySelectedNote = NoteAssetLoader.CustomNoteObjects.ElementAt(row).FileName;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            foreach (CustomNote note in NoteAssetLoader.CustomNoteObjects)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(note.NoteDescriptor.NoteName, note.NoteDescriptor.AuthorName, note.NoteDescriptor.Icon));
            }

            customListTableData.tableView.ReloadData();
            int selectedNote = NoteAssetLoader.SelectedNote;

            customListTableData.tableView.ScrollToCellWithIdx(selectedNote, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedNote); //(0, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
        }
    }
}
