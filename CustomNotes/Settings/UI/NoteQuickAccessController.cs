using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomNotes.Data;
using CustomNotes.Managers;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using CustomNotes.Utilities.MultiplayerCustomNoteModeExtensions;
using HMUI;
using UnityEngine;
using Zenject;

namespace CustomNotes.Settings.UI
{
    internal class NoteQuickAccessController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private PluginConfig _pluginConfig;
        private NoteAssetLoader _noteAssetLoader;

        [UIParams]
        BSMLParserParams parserParams = null;

        [Inject]
        public NoteQuickAccessController(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader) {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
        }

        [UIValue("multi-note-mode-list-options")]
        protected List<object> options = new object[]
        {
            MultiplayerCustomNoteMode.None.ToSettingsString(),
            MultiplayerCustomNoteMode.SameAsLocalPlayer.ToSettingsString(),
            MultiplayerCustomNoteMode.Random.ToSettingsString(),
            MultiplayerCustomNoteMode.RandomConsistent.ToSettingsString()
        }.ToList();

        [UIValue("multi-note-mode-list-choice")]
        protected string listChoice = null;

        [UIComponent("note-list")]
        protected CustomListTableData customListTableData = null;

        [UIComponent("scroll-indicator")]
        protected BSMLScrollIndicator scrollIndicator = null;

        private Coroutine _scrollIndicatorCoroutine = null;

        [UIAction("update-scroll-indicator-up")]
        protected void ScrollUp() => Utils.ScrollTheScrollIndicator(true, customListTableData.tableView, scrollIndicator, _scrollIndicatorCoroutine);

        [UIAction("update-scroll-indicator-down")]
        protected void ScrollDown() => Utils.ScrollTheScrollIndicator(false, customListTableData.tableView, scrollIndicator, _scrollIndicatorCoroutine);

        [UIAction("on-multi-note-mode-change")]
        protected void OnMultiNoteModeChange(string choice) {
            MultiplayerCustomNoteModeExtensions.SetMultiNoteSettingFromString(_pluginConfig, choice);
        }

        [UIAction("#post-parse")]
        protected void PostParse() {
            listChoice = MultiplayerCustomNoteModeExtensions.GetMultiNoteSettingString(_pluginConfig);
            parserParams.EmitEvent("multi-note-mode-update");
            SetupList();
            Utils.UpdateScrollIndicator(customListTableData.tableView, scrollIndicator);
        }

        protected void SetupList() {
            customListTableData.data.Clear();

            foreach (CustomNote note in _noteAssetLoader.CustomNoteObjects) {
                Sprite sprite = Sprite.Create(note.Descriptor.Icon, new Rect(0, 0, note.Descriptor.Icon.width, note.Descriptor.Icon.height), new Vector2(0.5f, 0.5f));
                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(note.Descriptor.NoteName, note.Descriptor.AuthorName, sprite);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            int selectedNote = _noteAssetLoader.SelectedNote;

            customListTableData.tableView.ScrollToCellWithIdx(selectedNote, TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedNote);
        }

        [UIAction("note-select")]
        protected void Select(TableView _, int row) {
            _noteAssetLoader.SelectedNote = row;
            _pluginConfig.LastNote = _noteAssetLoader.CustomNoteObjects[row].FileName;
        }

        public void ExternalNoteSelect(int row) {
            customListTableData.tableView.ScrollToCellWithIdx(row, TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(row);
            Utils.UpdateScrollIndicator(customListTableData.tableView, scrollIndicator);
        }

        public void ExternalReload() {
            SetupList();
        }

        public void Initialize() {

        }

        public void Dispose() {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            try {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            } catch { }
        }
    }
}
