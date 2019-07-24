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
        public static Material note { get; private set; }
        public static Material bomb { get; private set; }
        public static Material arrow { get; private set; }

        const string noteReplaceMatName = "_note_replace (Instance)";
        const string bombReplaceMatName = "_bomb_replace (Instance)";
        const string arrowReplaceMatName = "_arrow_replace (Instance)";

        public static void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            var materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Material test in materials)
            {
                Console.WriteLine("MATERIAL NAME");
                Console.WriteLine(test.name);
            }
            note = new Material(materials.First(x => x.name == "NoteHD"));
            arrow = new Material(materials.First(x => x.name == "NoteArrowHD"));
            if (materials.First(x => x.name == "BombNote"))
            {
                bomb = new Material(materials.First(x => x.name == "BombNote"));
            }
            //bomb = new Material(materials.First(x => x.name == "BombNote"));
        }

        public static void ReplaceMaterialsForGameObject(GameObject go)
        {
            if (note == null || bomb == null || arrow == null) GetMaterials();
            ReplaceAllMaterialsForGameObjectChildren(go, note, noteReplaceMatName);
            ReplaceAllMaterialsForGameObjectChildren(go, arrow, arrowReplaceMatName);
            if (bomb != null)
            {
                ReplaceAllMaterialsForGameObjectChildren(go, bomb, bombReplaceMatName);
            }
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
