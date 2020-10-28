using System.Reflection;

namespace CustomNotes.Overrides
{
    public class CustomNoteColorNoteVisuals : ColorNoteVisuals
    {
        public CustomNoteColorNoteVisuals()
        {
            ColorNoteVisuals original = GetComponent<ColorNoteVisuals>();
            foreach (FieldInfo info in original.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                info.SetValue(this, info.GetValue(original));
            }
            Destroy(original);
        }

        public void TurnOffVisuals()
        {
            _arrowMeshRenderer.enabled = false;
            _arrowGlowSpriteRenderer.enabled = false;
            _circleGlowSpriteRenderer.enabled = false;
        }
    }
}