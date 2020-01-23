using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomNotes.Data;
using CustomNotes.Utilities;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

        [UIComponent("note-description")]
        public TextPageScrollView noteDescription;

        public void OnNoteWasChanged(CustomNote customNote)
        {
            noteDescription.SetText($"{customNote.Descriptor.NoteName}:\n\n{Utils.SafeUnescape(customNote.Descriptor.Description)}");
        }
    }
}
