using CustomNotes.Utilities;
using IPA.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace CustomNotes.Overrides
{
    public class CustomNoteColorNoteVisuals : ColorNoteVisuals
    {
        public Color NoteColor
        {
            get { return ReflectionUtil.GetField<Color, ColorNoteVisuals>(this, "_noteColor"); }
            set { ReflectionUtil.SetField<ColorNoteVisuals, Color>(this, "_noteColor", value); }
        }

        private MeshRenderer[] ArrowMeshRenderers
        {
            get { return ReflectionUtil.GetField<MeshRenderer[], ColorNoteVisuals>(this, "_arrowMeshRenderers"); }
        }

        private MeshRenderer[] CircleMeshRenderers
        {
            get { return ReflectionUtil.GetField<MeshRenderer[], ColorNoteVisuals>(this, "_circleMeshRenderers"); }
        }

        private MaterialPropertyBlockController[] MaterialPropertyBlockControllers
        {
            get { return ReflectionUtil.GetField<MaterialPropertyBlockController[], ColorNoteVisuals>(this, "_materialPropertyBlockControllers"); }
        }

        public MeshRenderer[] ArrowObjects
        {
            get
            {
                List<MeshRenderer> arrowObjectList = new List<MeshRenderer>();
                arrowObjectList.AddRange(ArrowMeshRenderers);
                arrowObjectList.AddRange(CircleMeshRenderers);
                return arrowObjectList.ToArray();
            }
        }

        public List<GameObject> duplicatedArrows = new List<GameObject>();

        public void SetColor(Color color, bool updateMaterialBlocks)
        {
            NoteColor = color;
            //_arrowGlowSpriteRenderer.color = _noteColor.ColorWithAlpha(_arrowGlowIntensity);
            //_circleGlowSpriteRenderer.color = _noteColor;
            if (updateMaterialBlocks)
            {
                MaterialPropertyBlockController[] _materialPropertyBlockControllers = MaterialPropertyBlockControllers;
                foreach (MaterialPropertyBlockController materialPropertyBlockController in _materialPropertyBlockControllers)
                {
                    materialPropertyBlockController.materialPropertyBlock.SetColor("_Color", ColorExtensions.ColorWithAlpha(color, 1f));
                    materialPropertyBlockController.ApplyChanges();
                }
            }
        }

        public void TurnOffVisuals()
        {
            foreach (MeshRenderer arrowRenderer in ArrowObjects)
            {
                arrowRenderer.enabled = false;
            }
        }

        public void SetBaseGameVisualsLayer(int layer)
        {
            foreach (MeshRenderer arrowRenderer in ArrowObjects)
            {
                arrowRenderer.gameObject.layer = layer;
            }
        }

        public void CreateFakeVisuals(int layer)
        {
            ClearDuplicatedArrows();
            foreach (MeshRenderer arrowRenderer in ArrowObjects)
            {
                DuplicateIfExists(arrowRenderer.gameObject, layer);
            }
        }

        public void CreateAndScaleFakeVisuals(int layer, float scale)
        {
            ClearDuplicatedArrows();
            foreach (MeshRenderer arrowRenderer in ArrowMeshRenderers)
            {
                ScaleIfExists(arrowRenderer.gameObject, layer, scale, new Vector3(0, 0.1f, -0.3f));
            }
            foreach (MeshRenderer circleRenderer in CircleMeshRenderers)
            {
                ScaleIfExists(circleRenderer.gameObject, layer, scale, new Vector3(0, 0, -0.25f));
            }
        }

        public void ScaleVisuals(float scale)
        {
            Vector3 scaleVector = new Vector3(1, 1, 1) * scale;

            foreach (MeshRenderer arrowRenderer in ArrowMeshRenderers)
            {
                if (arrowRenderer.gameObject.name == "NoteArrowGlow") arrowRenderer.gameObject.transform.localScale = new Vector3(0.6f, 0.3f, 0.6f) * scale;
                else arrowRenderer.gameObject.transform.localScale = scaleVector;

                arrowRenderer.gameObject.transform.localPosition = new Vector3(0, 0.1f, -0.3f) * scale;
            }

            foreach (MeshRenderer circleRenderer in CircleMeshRenderers)
            {
                circleRenderer.gameObject.transform.localScale = scaleVector / 2;
                circleRenderer.gameObject.transform.localPosition = new Vector3(0, 0, -0.3f) * scale;
            }
        }

        private void ClearDuplicatedArrows()
        {
            for (int i = 0; i < duplicatedArrows.Count; i++)
            {
                duplicatedArrows[i].SetActive(false);
                Destroy(duplicatedArrows[i]);
            }
            duplicatedArrows.Clear();
        }

        private GameObject DuplicateIfExists(GameObject gameObject, int layer)
        {
            if (gameObject.activeInHierarchy)
            {
                GameObject tempObject = Instantiate(gameObject);
                tempObject.transform.parent = gameObject.transform.parent;
                tempObject.transform.localScale = gameObject.transform.localScale;
                tempObject.transform.localPosition = gameObject.transform.localPosition;
                LayerUtils.SetLayer(tempObject, layer);
                duplicatedArrows.Add(tempObject);
                return tempObject;
            }
            else return null;
        }

        private void ScaleIfExists(GameObject gameObject, int layer, float scale, Vector3 positionModifier)
        {
            GameObject tempObject = DuplicateIfExists(gameObject, layer);
            if (tempObject != null)
            {
                Vector3 scaleVector = new Vector3(1, 1, 1) * scale;

                tempObject.transform.localScale = scaleVector;

                tempObject.transform.localPosition = positionModifier * scale;
            }
        }
    }
}