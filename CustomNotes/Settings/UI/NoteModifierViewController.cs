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
using UnityEngine;

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

        [UIValue("notes-list")]
        private List<object> notesList = new List<object>();

        [UIComponent("notes-dropdown")]
        private DropDownListSetting notesDropdown;

        [UIComponent("notes-dropdown")]
        private RectTransform notesDropdownTransform;

        private RectTransform dropdownListTransform;

        [UIComponent("size-slider")]
        private SliderSetting sizeSlider;

        [UIComponent("hmd-checkbox")]
        private ToggleSetting hmdCheckbox;


        public NoteModifierViewController(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader)
        {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
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

        internal void ParentControllerActivated()
        {
            if (dropdownListTransform == null)
            {
                dropdownListTransform = notesDropdownTransform.Find("DropdownTableView").GetComponent<RectTransform>();
            }
            notesDropdown.ReceiveValue();
            sizeSlider.ReceiveValue();
            hmdCheckbox.ReceiveValue();
        }

        internal void ParentControllerDeactivated() // This is for fixing a weird bug with the dropdownlist
        {
            if (dropdownListTransform != null && notesDropdown != null)
            {
                dropdownListTransform.SetParent(notesDropdownTransform);
                dropdownListTransform.gameObject.SetActive(false);
            }
        }

        public void SetupList()
        {
            notesList = new List<object>();
            foreach (CustomNote note in _noteAssetLoader.CustomNoteObjects)
            {
                notesList.Add(note.Descriptor.NoteName);
            }
        }

        [UIAction("note-selected")]
        public void OnSelect(string selectedCell)
        {
            int selectedNote = _noteAssetLoader.CustomNoteObjects.ToList().FindIndex(note => note.Descriptor.NoteName == selectedCell);
            _noteAssetLoader.SelectedNote = selectedNote;
            _pluginConfig.LastNote = _noteAssetLoader.CustomNoteObjects[selectedNote].FileName;
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
            get { return _pluginConfig.NoteSize; }
            set
            {
                _pluginConfig.NoteSize = value;
            }
        }

        [UIValue("hmd-only")]
        public bool hmdOnly
        {
            get { return _pluginConfig.HMDOnly; }
            set
            {
                _pluginConfig.HMDOnly = value;
            }
        }
    }
}
