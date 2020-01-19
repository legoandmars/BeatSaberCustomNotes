using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomNotes.Data;
using CustomNotes.Utilities;
using TMPro;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

        [UIComponent("note-description")]
        public TextPageScrollView noteDescription;

        [UIComponent("score-submission-info")]
        public TextMeshProUGUI scoreSubmissionInfo;

        public void OnNoteWasChanged(CustomNote customNote)
        {
            noteDescription.SetText($"{customNote.Descriptor.NoteName}:\n\n{Utils.SafeUnescape(customNote.Descriptor.Description)}");
        }
    }
}
