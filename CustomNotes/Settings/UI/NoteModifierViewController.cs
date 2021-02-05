using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using CustomNotes.Settings.Utilities;
using CustomNotes.Managers;
using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using CustomNotes.Data;
using System.Collections.Generic;

namespace CustomNotes.Settings.UI
{
    /*
     * View Controller for the Custom Notes selection found under the Mods tab in the Modifiers View
     * Allows for hotswapping notes without going to main menu or leaving TA lobby
     */
    class NoteModifierViewController : IInitializable, IDisposable
    {
        private PluginConfig _pluginConfig;
        private NoteAssetLoader _noteAssetLoader;
        private NoteListViewController _noteListViewController;

        [UIValue("notes-list")]
        private List<object> notesList = new List<object>();

        [UIValue("selected-note")]
        private string selectedNote = "Default";

        [UIComponent("notes-dropdown")]
        private SimpleTextDropdown notesDropdown;

        public NoteModifierViewController(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, NoteListViewController noteListViewController)
        {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _noteListViewController = noteListViewController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Custom Notes", "CustomNotes.Settings.UI.Views.noteModifier.bsml", this);
            SetupList();
        }

        public void Dispose()
        {
            GameplaySetup.instance.RemoveTab("Custom Notes");
        }

        [UIAction("note-selected")]
        public void OnSelect(string selectedCell)
        {
            int selectedNote = notesDropdown.selectedIndex;
            _noteAssetLoader.SelectedNote = selectedNote;
            _pluginConfig.LastNote = _noteAssetLoader.CustomNoteObjects[selectedNote].FileName;
        }

        internal void OnNotesReloaded()
        {
            SetupList();
            notesDropdown.SelectCellWithIdx(_noteAssetLoader.SelectedNote); // Hacky way to get it to read the value of selectedNote
        }

        internal void OnNoteListSelected(CustomNote customNote)
        {
            selectedNote = customNote.Descriptor.name;
            notesDropdown.SelectCellWithIdx(_noteAssetLoader.SelectedNote);
        }

        public void SetupList()
        {
            notesList = new List<object>();
            foreach (CustomNote note in _noteAssetLoader.CustomNoteObjects)
            {
                notesList.Add(note.Descriptor.NoteName);
            }
            selectedNote = (string)notesList[_noteAssetLoader.SelectedNote];
        }
    }
}
