using CustomNotes.Utilities;
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

        public void CreateFakeVisuals(int layer)
        {
            DuplicateIfExists(_arrowMeshRenderer.gameObject, layer);
            DuplicateIfExists(_arrowGlowSpriteRenderer.gameObject, layer);
            DuplicateIfExists(_circleGlowSpriteRenderer.gameObject, layer);
        }

        public void CreateAndScaleFakeVisuals(int layer, float scale)
        {
            ScaleIfExists(_arrowMeshRenderer.gameObject, layer, scale, new Vector3(0, 0.11f, -0.25f));
            ScaleIfExists(_arrowGlowSpriteRenderer.gameObject, layer, scale, new Vector3(0, 0.11f, -0.25f));
            ScaleIfExists(_circleGlowSpriteRenderer.gameObject, layer, scale, new Vector3(0, 0, -0.25f));
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

        private GameObject DuplicateIfExists(GameObject gameObject, int layer)
        {
            if (gameObject.activeInHierarchy)
            {
                GameObject tempObject = Instantiate(gameObject);
                tempObject.transform.parent = gameObject.transform.parent;
                LayerUtils.SetLayer(tempObject, layer);
                return tempObject;
            }
            else return null;
        }

        private void ScaleIfExists(GameObject gameObject, int layer, float scale, Vector3 positionModifier) {
            GameObject tempObject = DuplicateIfExists(gameObject, layer);
            if(tempObject != null)
            {
                Vector3 scaleVector = new Vector3(1, 1, 1) * scale;

                tempObject.transform.localScale = scaleVector;

                tempObject.transform.localPosition = positionModifier * scale;
            }
        }
    }
}