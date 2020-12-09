using UnityEngine;
using System.Collections.Generic;

namespace CustomNotes.Utilities
{
    internal class MaterialSwapper
    {
        public static IEnumerable<Material> AllMaterials { get; private set; }

        public static void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            AllMaterials = Resources.FindObjectsOfTypeAll<Material>();
        }

        public static void ReplaceMaterialsForGameObject(GameObject gameObject)
        {
            if (AllMaterials == null)
            {
                GetMaterials();
            }

            foreach (Material currentMaterial in AllMaterials)
            {
                string materialName = currentMaterial.name.ToLower() + "_replace (Instance)";
                ReplaceAllMaterialsForGameObjectChildren(gameObject, currentMaterial, materialName);
            }
        }

        public static void ReplaceAllMaterialsForGameObjectChildren(GameObject gameObject, Material material, string materialToReplaceName = "")
        {
            IEnumerable<Renderer> renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                ReplaceAllMaterialsForGameObject(renderer.gameObject, material, materialToReplaceName);
            }
        }

        public static void ReplaceAllMaterialsForGameObject(GameObject gameObject, Material material, string materialToReplaceName = "")
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            Material[] materialsCopy = renderer.materials;
            bool materialsDidChange = false;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (materialsCopy[i].name.Equals(materialToReplaceName) || materialToReplaceName == "")
                {
                    Color oldColor = materialsCopy[i].GetColor("_Color");
                    materialsCopy[i] = material;
                    materialsCopy[i].SetColor("_Color", oldColor);
                    materialsDidChange = true;
                }
            }

            if (materialsDidChange)
            {
                renderer.materials = materialsCopy;
            }
        }
    }
}
