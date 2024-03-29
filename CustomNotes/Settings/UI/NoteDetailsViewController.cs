﻿using HMUI;
using CustomNotes.Data;
using CustomNotes.Utilities;
using CustomNotes.Settings.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;
using CustomNotes.Settings.UI;
using BeatSaberMarkupLanguage.Components.Settings;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

        private PluginConfig _pluginConfig;
        private NoteListViewController _listViewController;

        [UIComponent("note-description")]
        public TextPageScrollView noteDescription = null;

        [UIComponent("disable-april-fools")]
        public ToggleSetting disableAprilFoolsCheckbox = null;

        public void OnNoteWasChanged(CustomNote customNote)
        {
            if (string.IsNullOrWhiteSpace(customNote.ErrorMessage))
            {
                noteDescription.SetText($"{customNote.Descriptor.NoteName}:\n\n{Utils.SafeUnescape(customNote.Descriptor.Description)}");
            }
            else
            {
                noteDescription.SetText(string.Empty);
            }

            NotifyPropertyChanged(nameof(modEnabled));
            NotifyPropertyChanged(nameof(noteSize));
            NotifyPropertyChanged(nameof(hmdOnly));
            NotifyPropertyChanged(nameof(autoDisable));
        }

        [Inject]
        public void Construct(PluginConfig pluginConfig, NoteListViewController listViewController)
        {
            _pluginConfig = pluginConfig;
            _listViewController = listViewController;
        }

        [UIValue("mod-enabled")]
        public bool modEnabled
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                NotifyPropertyChanged(nameof(modEnabled));
            }
        }

        [UIValue("note-size")]
        public float noteSize
        {
            get => _pluginConfig.NoteSize;
            set 
            { 
                _pluginConfig.NoteSize = value;
                _listViewController.ScalePreviewNotes(value);
                NotifyPropertyChanged(nameof(noteSize));
            }
        }

        [UIValue("hmd-only")]
        public bool hmdOnly 
        {
            get => _pluginConfig.HMDOnly;
            set 
            { 
                _pluginConfig.HMDOnly = value;
                NotifyPropertyChanged(nameof(hmdOnly));
            }
        }

        [UIValue("auto-disable")]
        public bool autoDisable
        {
            get => _pluginConfig.AutoDisable;
            set
            {
                _pluginConfig.AutoDisable = value;
                NotifyPropertyChanged(nameof(autoDisable));
            }
        }

        [UIValue("disable-april-fools")]
        public bool disableAprilFools
        {
            get => _pluginConfig.DisableAprilFools;
            set
            {
                _pluginConfig.DisableAprilFools = value;
                NotifyPropertyChanged(nameof(disableAprilFools));
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            // Not April Fools Day Code
            System.DateTime time;
            bool bunbundai = false;
            if (IPA.Utilities.Utils.CanUseDateTimeNowSafely)
                time = System.DateTime.Now;
            else
                time = System.DateTime.UtcNow;

            disableAprilFoolsCheckbox.gameObject.SetActive((time.Month == 4 && time.Day == 1) || bunbundai);
        }
    }
}
