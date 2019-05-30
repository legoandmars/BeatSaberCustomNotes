using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace CustomNotes
{
    public class NoteDescriptor : MonoBehaviour
    {
        public string NoteName = "Note";
        public string AuthorName = "Author";
        public Texture2D Icon;
        public bool DisableBaseNoteArrows = false;
    }
}
