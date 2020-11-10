using System.Reflection;
using UnityEngine;

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

        public void SetColor(Color color, bool updateMaterialBlocks)
        {
            _noteColor = color;
            _arrowGlowSpriteRenderer.color = _noteColor.ColorWithAlpha(_arrowGlowIntensity);
            _circleGlowSpriteRenderer.color = _noteColor;
            if (updateMaterialBlocks)
            {
                foreach (MaterialPropertyBlockController materialPropertyBlockController in _materialPropertyBlockControllers)
                {
                    materialPropertyBlockController.materialPropertyBlock.SetColor(ColorNoteVisuals._colorId, _noteColor.ColorWithAlpha(1f));
                    materialPropertyBlockController.ApplyChanges();
                }
            }
        }

        public void TurnOffVisuals()
        {
            _arrowMeshRenderer.enabled = false;
            _arrowGlowSpriteRenderer.enabled = false;
            _circleGlowSpriteRenderer.enabled = false;
        }
    }
}