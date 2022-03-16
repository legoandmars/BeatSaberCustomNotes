using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using CustomNotes.Settings.Utilities;
using CustomNotes.Managers;
using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using CustomNotes.Data;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components.Settings;
using System.Linq;
using System.ComponentModel;
using BeatSaberMarkupLanguage;

namespace CustomNotes.Settings.UI
{
    /*
     * View Controller for the Custom Notes selection found under the Mods tab in the Modifiers View
     * Allows for hotswapping notes without going to main menu or leaving TA lobby
     */
    internal class NoteModifierViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private PluginConfig _pluginConfig;
        private NoteAssetLoader _noteAssetLoader;

        [UIValue("notes-list")]
        private List<object> notesList = new List<object>();

        [UIComponent("notes-dropdown")]
        private DropDownListSetting notesDropdown = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public NoteModifierViewController(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader)
        {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
        }

        public void Initialize()
        {
            SetupList();
            GameplaySetup.instance.AddTab("Custom Notes", "CustomNotes.Settings.UI.Views.noteModifier.bsml", this);
        }

        public void Dispose()
        {
            if (GameplaySetup.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
                GameplaySetup.instance.RemoveTab("Custom Notes");
        }

        internal void ParentControllerActivated()
        {
            notesDropdown.ReceiveValue();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(modEnabled)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(noteSize)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(hmdOnly)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(autoDisable)));
        }

        public void SetupList()
        {
            notesList = new List<object>();
            foreach (CustomNote note in _noteAssetLoader.CustomNoteObjects)
            {
                notesList.Add(note.Descriptor.NoteName);
            }

            if (notesDropdown != null)
            {
                notesDropdown.values = notesList;
                notesDropdown.UpdateChoices();
            }
        }

        [UIAction("note-selected")]
        public void OnSelect(string selectedCell)
        {
            int selectedNote = _noteAssetLoader.CustomNoteObjects.ToList().FindIndex(note => note.Descriptor.NoteName == selectedCell);
            _noteAssetLoader.SelectedNote = selectedNote;
            _noteAssetLoader.SelectedBomb = selectedNote;
            _pluginConfig.LastNote = _noteAssetLoader.CustomNoteObjects[selectedNote].FileName;
            _pluginConfig.LastBomb = _noteAssetLoader.CustomNoteObjects[selectedNote].FileName;
        }

        [UIValue("mod-enabled")]
        public bool modEnabled
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(modEnabled)));
            }
        }

        [UIValue("selected-note")]
        private string selectedNote
        {
            get
            {
                if (_noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote].ErrorMessage != null) // Only select if valid bloq is loaded
                {
                    return _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote].Descriptor.NoteName;
                }
                return "Default";
            }
        }

        [UIValue("note-size")]
        public float noteSize
        {
            get => _pluginConfig.NoteSize;
            set
            {
                _pluginConfig.NoteSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(noteSize)));
            }
        }

        [UIValue("hmd-only")]
        public bool hmdOnly
        {
            get => _pluginConfig.HMDOnly;
            set
            {
                _pluginConfig.HMDOnly = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(hmdOnly)));
            }
        }

        [UIValue("auto-disable")]
        public bool autoDisable
        {
            get => _pluginConfig.AutoDisable;
            set
            {
                _pluginConfig.AutoDisable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(autoDisable)));
            }
        }
    }
}
