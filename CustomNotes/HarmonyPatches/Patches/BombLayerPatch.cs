using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Zenject;
using CustomNotes.Managers;
using CustomNotes.Utilities;

namespace CustomNotes.HarmonyPatches
{
    [HarmonyPatch(typeof(BombNoteController))]
    [HarmonyPatch("Init", MethodType.Normal)]
    internal class BombInitPatch
    {
        private static void Prefix(ref BombNoteController __instance)
        {
            // I don't like this solution. I want to remove it.
            // However, I don't know how to make this properly work with the current system
            // This basically just does what pre-zenject customnotes did
            // It creates a duplicate of the bomb mesh, and sets it to the first-person layer
            // If this isn't done, bombs won't be visible in-hmd, which is pretty important.

            if (LayerUtils.BombPatchRequired)
            {
                Transform child = __instance.gameObject.transform.Find("Mesh");
                if (!child.transform.parent.Find("FirstPersonBomb"))
                {

                    GameObject tempObject = UnityEngine.Object.Instantiate(child.gameObject);
                    tempObject.name = "FirstPersonBomb";
                    tempObject.transform.parent = child.parent;

                    tempObject.transform.localScale = child.localScale;
                    tempObject.transform.localPosition = child.localPosition;

                    LayerUtils.SetLayer(tempObject, LayerUtils.NoteLayer.FirstPerson);
                }
            }
        }
    }
}