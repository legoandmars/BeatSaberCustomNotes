using System;
using HarmonyLib;
using UnityEngine;
using CustomNotes.Data;
using CustomNotes.Utilities;

namespace CustomNotes.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorNoteVisuals))]
    [HarmonyPatch("HandleNoteControllerDidInit", MethodType.Normal)]
    internal class ColorNoteVisualsPatch
    {
        private static void Prefix(NoteController noteController, ref ColorManager ____colorManager)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote];

                if (activeNote.FileName != "DefaultNotes")
                {
                    // Attempt to find both the custom ArrowNote and the custom DotNote
                    Transform child = noteController.gameObject.transform.Find("NoteCube");

                    GameObject customNote = null;
                    GameObject recolorizeCustomNote = null;
                    string name = string.Empty;

                    switch (noteController.noteData.colorType)
                    {
                        case ColorType.ColorA:
                            customNote = GetCustomNote(child, activeNote.NoteLeft, activeNote.NoteDotLeft, noteController.noteData.cutDirection, out name, out recolorizeCustomNote);
                            break;
                        case ColorType.ColorB:
                            customNote = GetCustomNote(child, activeNote.NoteRight, activeNote.NoteDotRight, noteController.noteData.cutDirection, out name, out recolorizeCustomNote);
                            break;
                        default:
                            break;
                    }

                    if (customNote != null)
                    {
                        // Custom Note of type Arrow/Dot not spawned yet for new default note object
                        GameObject fakeMesh = UnityEngine.Object.Instantiate(customNote);
                        fakeMesh.name = name;
                        fakeMesh.transform.SetParent(child);
                        fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                        fakeMesh.transform.localRotation = fakeMesh.transform.localRotation * child.parent.localRotation; // correct for 360 levels

                        if (activeNote.Descriptor.UsesNoteColor)
                        {
                            Utils.ColorizeCustomNote(____colorManager.ColorForType(noteController.noteData.colorType), activeNote.Descriptor.NoteColorStrength, fakeMesh);
                        }
                    }
                    else if(activeNote.Descriptor.UsesNoteColor && recolorizeCustomNote != null)
                        Utils.ColorizeCustomNote(____colorManager.ColorForType(noteController.noteData.colorType), activeNote.Descriptor.NoteColorStrength, recolorizeCustomNote);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        private static void Postfix(NoteController noteController, ref MeshRenderer ____arrowMeshRenderer, ref SpriteRenderer ____arrowGlowSpriteRenderer, ref SpriteRenderer ____circleGlowSpriteRenderer)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote];

                if (activeNote.FileName != "DefaultNotes")
                {
                    // Check and activate/deactivate custom note objects attached to a pooled note object
                    if (noteController.noteData.colorType == ColorType.ColorA
                        || noteController.noteData.colorType == ColorType.ColorB)
                    {
                        Transform child = noteController.gameObject.transform.Find("NoteCube");
                        GameObject oldCustomNote = child.Find("customNoteArrow")?.gameObject;
                        GameObject oldCustomNoteDot = child.Find("customNoteDot")?.gameObject;

                        if (noteController.noteData.cutDirection == NoteCutDirection.Any)
                        {
                            Utils.ActivateGameObject(oldCustomNoteDot, true, oldCustomNote);
                        }
                        else
                        {
                            Utils.ActivateGameObject(oldCustomNote, true, oldCustomNoteDot);
                        }
                    }

                    // Hide certain parts of the default note which is not required
                    MeshRenderer noteMesh = noteController.gameObject.GetComponentInChildren<MeshRenderer>();
                    //noteMesh.enabled = false;
                    noteMesh.forceRenderingOff = true;

                    if (activeNote.Descriptor.DisableBaseNoteArrows)
                    {
                        ____arrowMeshRenderer.enabled = false;
                        ____arrowGlowSpriteRenderer.enabled = false;
                        ____circleGlowSpriteRenderer.enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        /// <summary>
        /// Gets a custom arrow or dot note depending on the requirements. Returns null if custom note is already attached to a parent note.
        /// </summary>
        /// <param name="noteChild">Parent note</param>
        /// <param name="noteArrow">Custom Arrow Note</param>
        /// <param name="noteDot">Custom Dot Note</param>
        /// <param name="noteCutDirection">The cut direction of the noteChild</param>
        /// <param name="name">Custom Note name</param>
        /// <returns></returns>
        private static GameObject GetCustomNote(Transform noteChild, GameObject noteArrow, GameObject noteDot, NoteCutDirection noteCutDirection, out string name, out GameObject recolorizeBlock)
        {
            name            = "DuplicateOrInvalid";
            recolorizeBlock = null;

            GameObject customNote = null;

            Transform dotObject = noteCutDirection == NoteCutDirection.Any ? noteChild.Find("customNoteDot") : null;
            Transform arrObject = dotObject == null ? noteChild.Find("customNoteArrow") : null;

            if (noteCutDirection == NoteCutDirection.Any)
            {
                // CustomNote Dot not attached to childNote
                if (dotObject == null || dotObject.gameObject == null)
                {
                    name        = "customNoteDot";
                    customNote  = noteDot ?? noteArrow;
                }
                else
                    recolorizeBlock = dotObject.gameObject;
            }
            else
            {
                // CustomNote Arrow not attached to childNote
                if (arrObject == null || arrObject.gameObject == null)
                {
                    name = "customNoteArrow";
                    customNote = noteArrow;
                }
                else
                    recolorizeBlock = arrObject.gameObject;
            }

            return customNote;
        }
    }

    [HarmonyPatch(typeof(BombNoteController))]
    [HarmonyPatch("Init", MethodType.Normal)]
    internal class BombInitPatch
    {
        private static void Prefix(ref BombNoteController __instance)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote];
                Transform child = __instance.gameObject.transform.Find("Mesh");

                // Only instantiate a new CustomNote if one is not already attached to this object
                // and we are not using the default ones
                if (activeNote.FileName != "DefaultNotes"
                    && activeNote.NoteBomb
                    && !child.Find("customNote")?.gameObject)
                {
                    GameObject fakeMesh = UnityEngine.Object.Instantiate(activeNote.NoteBomb);
                    fakeMesh.name = "customNote";
                    fakeMesh.transform.SetParent(child);
                    fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    fakeMesh.transform.Rotate(new Vector3(0f, 0f, 90f), Space.Self);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        private static void Postfix(ref BombNoteController __instance)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote];

                if (activeNote.FileName != "DefaultNotes"
                    && activeNote.NoteBomb)
                {
                    // Hide certain parts of the default bomb which is not required
                    MeshRenderer bombMesh = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
                    bombMesh.enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }
    }
}
