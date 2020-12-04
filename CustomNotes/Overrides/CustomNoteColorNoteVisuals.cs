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

        public void ScaleVisuals(float scale)
        {
            Vector3 scaleVector = new Vector3(1, 1, 1) * scale;
            _arrowMeshRenderer.gameObject.transform.localScale = scaleVector;
            _arrowGlowSpriteRenderer.gameObject.transform.localScale = scaleVector;
            _circleGlowSpriteRenderer.gameObject.transform.localScale = scaleVector;

            _arrowMeshRenderer.gameObject.transform.localPosition = new Vector3(0, 0.11f, -0.25f) * scale;
            _arrowGlowSpriteRenderer.gameObject.transform.localPosition = new Vector3(0, 0.11f, -0.25f) * scale;
            _circleGlowSpriteRenderer.gameObject.transform.localPosition = new Vector3(0, 0, -0.25f) * scale;
        }
    }
}