using HMUI;
using CustomNotes.Data;
using CustomNotes.Utilities;
using CustomNotes.Managers;
using CustomNotes.Settings.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;
using System;
using CustomNotes.Settings.UI;
using System.Collections.Generic;
using System.Linq;
using CustomNotes.Utilities.MultiplayerCustomNoteModeExtensions;
using BeatSaberMarkupLanguage.Parser;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

        private PluginConfig _pluginConfig;
        private NoteListViewController _listViewController;

        [UIParams]
        BSMLParserParams parserParams = null;

        [UIComponent("note-description")]
        public TextPageScrollView noteDescription = null;

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

        [UIValue("multi-note-mode-list-options")]
        private List<object> options = new object[]
        {
            MultiplayerCustomNoteMode.None.ToSettingsString(),
            MultiplayerCustomNoteMode.SameAsLocalPlayer.ToSettingsString(),
            MultiplayerCustomNoteMode.Random.ToSettingsString(),
            MultiplayerCustomNoteMode.RandomConsistent.ToSettingsString()
        }.ToList();

        [UIValue("multi-note-mode-list-choice")]
        protected string listChoice = null;

        [UIAction("on-multi-note-mode-change")]
        protected void OnMultiNoteModeChange(string choice)
        {
            MultiplayerCustomNoteModeExtensions.SetMultiNoteSettingFromString(_pluginConfig, choice);
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            listChoice = MultiplayerCustomNoteModeExtensions.GetMultiNoteSettingString(_pluginConfig);
            parserParams.EmitEvent("multi-note-mode-update");
        }
    }
}
