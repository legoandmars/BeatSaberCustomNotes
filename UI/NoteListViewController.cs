using HMUI;
using TMPro;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using CustomNotes.Utilities;

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

        public override TableCell CellForIdx(int idx)
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
