using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using CustomNotes.Utilities;
using System;
namespace CustomNotes.UI
{
    class NotesListView : BeatSaberMarkupLanguage.ViewControllers.BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.UI.noteList.bsml";
        [UIComponent("noteList")]
        public CustomListTableData customListTableData;



        [UIAction("noteSelect")]
        internal void SelectSaber(TableView tableView, int row)
        {
            NoteAssetLoader.selectedNote = row;
            Configuration.CurrentlySelectedNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote].FileName;

        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
        }

        [UIAction("#post-parse")]
        internal void SetupList()
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

/*
namespace CustomNotes.UI
{
    
    public class NoteListViewController : CustomListViewController
    {
        public override void __Activate(ActivationType activationType)
        {
            base.__Activate(activationType);
            _customListTableView.SelectCellWithIdx(NoteAssetLoader.selectedNote);
        }

        public override int NumberOfCells()
        {
            return NoteAssetLoader.customNotes.Length;
        }

        public override TableCell CellForIdx(TableView tableView, int idx)
        {
            CustomNote note = NoteAssetLoader.customNotes[idx];

            LevelListTableCell _tableCell = GetTableCell(false);
            _tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = note.NoteDescriptor.NoteName;
            _tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = note.NoteDescriptor.AuthorName;
            _tableCell.GetPrivateField<UnityEngine.UI.RawImage>("_coverRawImage").texture = note.NoteDescriptor.Icon ?? UnityEngine.Texture2D.blackTexture;
            _tableCell.reuseIdentifier = "CustomNoteListCell";

            return _tableCell;
        }
    }
    
}
*/