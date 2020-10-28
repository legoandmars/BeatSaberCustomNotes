using HMUI;
using CustomNotes.Data;
using CustomNotes.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

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
    }
}
