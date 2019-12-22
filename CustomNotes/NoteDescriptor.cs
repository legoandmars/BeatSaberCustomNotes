using UnityEngine;

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
