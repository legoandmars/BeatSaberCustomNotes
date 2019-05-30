using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using TMPro;

namespace CustomNotes.UI
{
    class NoteListViewController : CustomListViewController
    {
        public override void __Activate(ActivationType activationType)
        {
            base.__Activate(activationType);
            _customListTableView.SelectCellWithIdx(Plugin.selectedNote);
        }

        public override int NumberOfCells()
        {
            return Plugin.customNotes.Length;
        }

        public override TableCell CellForIdx(int idx)
        {
            CustomNote note = Plugin.customNotes[idx];
            LevelListTableCell _tableCell = GetTableCell(false);
            _tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = note.noteDescriptor.NoteName;
            _tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = note.noteDescriptor.AuthorName;
            _tableCell.GetPrivateField<UnityEngine.UI.RawImage>("_coverRawImage").texture = note.noteDescriptor.Icon ?? UnityEngine.Texture2D.blackTexture;
            _tableCell.reuseIdentifier = "CustomNoteListCell";
            return _tableCell;
        }




    }
}
