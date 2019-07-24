using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace CustomNotes
{
    class MaterialSwapper
    {
        public static Material[] allMaterials { get; private set; }

        public static void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        }

        public static void ReplaceMaterialsForGameObject(GameObject go)
        {
            if (allMaterials == null) GetMaterials();
            foreach(Material currentMaterial in allMaterials)
            {
                string materialName = currentMaterial.name.ToLower()+"_replace (Instance)";
                ReplaceAllMaterialsForGameObjectChildren(go, currentMaterial, materialName);
            }
            /*ReplaceAllMaterialsForGameObjectChildren(go, note, noteReplaceMatName);
            ReplaceAllMaterialsForGameObjectChildren(go, arrow, arrowReplaceMatName);
            if (bomb != null)
            {
                ReplaceAllMaterialsForGameObjectChildren(go, bomb, bombReplaceMatName);
            }*/
        }

        public static void ReplaceAllMaterialsForGameObjectChildren(GameObject go, Material mat, string matToReplaceName = "")
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
            {
                ReplaceAllMaterialsForGameObject(r.gameObject, mat, matToReplaceName);
            }
        }

        public static void ReplaceAllMaterialsForGameObject(GameObject go, Material mat, string matToReplaceName = "")
        {
            Renderer r = go.GetComponent<Renderer>();
            Material[] materialsCopy = r.materials;
            bool materialsDidChange = false;

            for (int i = 0; i < r.materials.Length; i++)
            {
                if (materialsCopy[i].name.Equals(matToReplaceName) || matToReplaceName == "")
                {
                    Color oldColor = materialsCopy[i].GetColor("_Color");
                    materialsCopy[i] = mat;
                    materialsCopy[i].SetColor("_Color", oldColor);
                    materialsDidChange = true;
                }
            }
            if (materialsDidChange)
            {
                r.materials = materialsCopy;
            }
        }
    }
}
