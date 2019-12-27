using UnityEngine;

// Class has to be in this namespace due to compatibility
namespace CustomNotes
{
    public class NoteDescriptor : MonoBehaviour
    {
        public string NoteName = "Note";
        public string AuthorName = "Author";
        public Texture2D Icon;
        public bool DisableBaseNoteArrows = false;
        public bool UsesNoteColor = false;
        public float NoteColorStrength = 1.0f;
    }
}
