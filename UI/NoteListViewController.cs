using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using TMPro;
using UnityEngine;

namespace CustomNotes.UI
{
    class NoteListViewController : CustomListViewController
    {
        private int selected = 0;

       /* private bool CustomColorsPresent = IPA.Loader.PluginManager.Plugins.Any(x => x.Name == "CustomColorsEdit" || x.Name == "Custom Colors")
            || IPA.Loader.PluginManager.AllPlugins.Any(x => x.Metadata.Id == "Custom Colors");
            */
        private MenuShockwave menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();

        private bool menuShockwaveOriginalState;

        public override void __Activate(ActivationType activationType)
        {
            Console.WriteLine("STARTING THIS SHIT");
            for (var i = 0; i < Plugin.customNotes.Length; i++)
            {
                if (i == Plugin.selectedNote)
                {
                    selected = i;
                }
            }

            base.__Activate(activationType);
            _customListTableView.SelectCellWithIdx(selected);

            if (_backButton == null)
            {
                _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);
                _backButton.onClick.AddListener(delegate ()
                {
                    backButtonPressed?.Invoke();

                    NotePreviewController.Instance.DestroyPreview();
                    _customListTableView.didSelectCellWithIdxEvent -= _sabersTableView_DidSelectRowEvent;
                    if (menuShockwave)
                    {
                        menuShockwave.enabled = menuShockwaveOriginalState;
                    }

                    /*if (CustomColorsPresent)
                    {
                        NotePreviewController.Instance.CallCustomColors(false);
                    }*/
                });
            }

            PreviewCurrent();

            if (menuShockwave)
            {
                menuShockwaveOriginalState = menuShockwave.enabled;
                menuShockwave.enabled = false;
            }

            /*var _versionNumber = BeatSaberUI.CreateText(rectTransform, "Text", new Vector2(-10f, -10f));

            (_versionNumber.transform as RectTransform).anchoredPosition = new Vector2(-10f, 10f);
            (_versionNumber.transform as RectTransform).anchorMax = new Vector2(1f, 0f);
            (_versionNumber.transform as RectTransform).anchorMin = new Vector2(1f, 0f);

            _versionNumber.text = $"v{Plugin.PluginVersion}";
            _versionNumber.fontSize = 5;
            _versionNumber.color = Color.white;
            */
            _customListTableView.didSelectCellWithIdxEvent += _sabersTableView_DidSelectRowEvent;
            _customListTableView.ScrollToCellWithIdx(Plugin.selectedNote, TableView.ScrollPositionType.Beginning, false);
            _customListTableView.SelectCellWithIdx(Plugin.selectedNote, false);
        }

        protected override void DidDeactivate(DeactivationType type) => base.DidDeactivate(type);

        private void PreviewCurrent()
        {
            if (selected != 0)
            {
                NotePreviewController.Instance.GeneratePreview(selected);
            }
            else
            {
                NotePreviewController.Instance.DestroyPreview();
            }
        }

        public override float CellSize() => 8.5f;

        public override int NumberOfCells() => Plugin.customNotes.Length;

        private void _sabersTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Plugin.selectedNote = row;
            selected = row;
            if (row == 0)
            {
                NotePreviewController.Instance.DestroyPreview();
            }
            else
            {
                NotePreviewController.Instance.GeneratePreview(row);
            }
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
        /*
        public override void __Activate(ActivationType activationType)
        {
            Console.WriteLine("new dfsa");
            base.__Activate(activationType);
            _customListTableView.SelectCellWithIdx(Plugin.selectedNote);
            _customListTableView.didSelectCellWithIdxEvent += _noteTableView_DidSelectRowEvent;
        }

        public void Deselect()
        {
            _customListTableView.didSelectCellWithIdxEvent -= _noteTableView_DidSelectRowEvent;
        }
        public override int NumberOfCells()
        {
            return Plugin.customNotes.Length;
        }
        private void _noteTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Console.WriteLine("new stuff");
            NotePreviewController.Instance.DoStuff();
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
        */



    }
}
