using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using CustomNotes.Utilities;
using HMUI;

namespace CustomNotes.UI
{
    internal class NotesListView : BeatSaberMarkupLanguage.ViewControllers.BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.UI.noteList.bsml";

        [UIComponent("noteList")]
        public CustomListTableData customListTableData;

        [UIAction("noteSelect")]
        public void Select(TableView tableView, int row)
        {
            NoteAssetLoader.selectedNote = row;
            Configuration.CurrentlySelectedNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote].FileName;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            foreach (CustomNote note in NoteAssetLoader.customNotes)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(note.NoteDescriptor.NoteName, note.NoteDescriptor.AuthorName, note.NoteDescriptor.Icon));
            }

            customListTableData.tableView.ReloadData();
            int selectedNote = NoteAssetLoader.selectedNote;

            customListTableData.tableView.ScrollToCellWithIdx(selectedNote, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedNote); //(0, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
        }
    }
}
