using HMUI;
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

        [UIComponent("size-slider")]
        private SliderSetting sizeSlider;

        [UIComponent("hmd-checkbox")]
        private ToggleSetting hmdCheckbox;

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

            if (sizeSlider != null)
            {
                sizeSlider.ReceiveValue();
            }
            if (hmdCheckbox != null)
            {
                hmdCheckbox.ReceiveValue();
            }
        }

        [Inject]
        public void Construct(PluginConfig pluginConfig, NoteListViewController listViewController)
        {
            _pluginConfig = pluginConfig;
            _listViewController = listViewController;
        }

        [UIValue("note-size")]
        public float noteSize
        {
            get { return _pluginConfig.NoteSize; }
            set 
            { 
                _pluginConfig.NoteSize = value;
                _listViewController.ScalePreviewNotes(value);
            }
        }

        [UIValue("hmd-only")]
        public bool hmdOnly 
        {
            get { return _pluginConfig.HMDOnly; }
            set 
            { _pluginConfig.HMDOnly = value; }
        }
    }
}
