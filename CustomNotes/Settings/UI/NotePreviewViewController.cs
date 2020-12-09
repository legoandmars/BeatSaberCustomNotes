using HMUI;
using CustomNotes.Data;
using CustomNotes.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CustomNotes.Settings.UI
{
    internal class NotePreviewViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.notePreview.bsml";

        [UIComponent("error-description")]
        public TextPageScrollView errorDescription = null;

        public void OnNoteWasChanged(CustomNote customNote)
        {
            if (!string.IsNullOrWhiteSpace(customNote.ErrorMessage))
            {
                errorDescription.gameObject.SetActive(true);
                errorDescription.SetText($"{customNote.Descriptor?.NoteName}:\n\n{Utils.SafeUnescape(customNote.ErrorMessage)}");
            }
            else
            {
                errorDescription.gameObject.SetActive(false);
            }
        }
    }
}