using System;
using Harmony;
using UnityEngine;
using CustomNotes.Utilities;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorNoteVisuals))]
    [HarmonyPatch("HandleNoteControllerDidInitEvent", MethodType.Normal)]
    internal class ColorNoteVisualsPatch
    {
        internal static void Prefix(NoteController noteController, ref ColorManager ____colorManager)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote];
                Transform child = noteController.gameObject.transform.Find("NoteCube");

                if (activeNote.FileName != "DefaultNotes")
                {
                    // Attempt to find both the custom ArrowNote and the custom DotNote
                    GameObject oldCustomNote = child.Find("customNoteArrow")?.gameObject;
                    GameObject oldCustomNoteDot = child.Find("customNoteDot")?.gameObject;

                    GameObject customNote;
                    string name = "customNoteArrow";
                    switch (noteController.noteData.noteType)
                    {
                        case NoteType.NoteA:
                            if (noteController.noteData.cutDirection == NoteCutDirection.Any)
                            {
                                if (oldCustomNoteDot)
                                {
                                    return; // Custom "Dot" note already attached to pooled Note GameObject
                                }

                                customNote = activeNote.NoteDotLeft ?? activeNote.NoteLeft;
                                name = "customNoteDot";
                            }
                            else
                            {
                                if (oldCustomNote)
                                {
                                    return; // Custom "Arrow" note already attached to pooled Note GameObject
                                }

                                customNote = activeNote.NoteLeft;
                            }

                            break;
                        case NoteType.NoteB:
                            if (noteController.noteData.cutDirection == NoteCutDirection.Any)
                            {
                                if (oldCustomNoteDot)
                                {
                                    return; // Custom "Dot" note already attached to pooled Note GameObject
                                }

                                customNote = activeNote.NoteDotRight ?? activeNote.NoteRight;
                                name = "customNoteDot";
                            }
                            else
                            {
                                if (oldCustomNote)
                                {
                                    return; // Custom "Arrow" note already attached to pooled Note GameObject
                                }

                                customNote = activeNote.NoteRight;
                            }

                            break;
                        default:
                            return;
                    }

                    if (activeNote.NoteDescriptor.UsesNoteColor)
                    {
                        Color noteColor = ____colorManager.ColorForNoteType(noteController.noteData.noteType) * activeNote.NoteDescriptor.NoteColorStrength;

                        for (int i = 0; i < customNote.GetComponentsInChildren<Transform>().Length; i++)
                        {
                            DisableNoteColorOnGameobject colorDisabled = customNote.GetComponentsInChildren<Transform>()[i].GetComponent<DisableNoteColorOnGameobject>();
                            if (!colorDisabled)
                            {
                                Renderer childRenderer = customNote.GetComponentsInChildren<Transform>()[i].GetComponent<Renderer>();
                                if (childRenderer)
                                {
                                    childRenderer.material.SetColor("_Color", noteColor);
                                }
                            }
                        }
                    }

                    // Custom Note of type Arrow/Dot not spawned yet for new default note object
                    GameObject fakeMesh = UnityEngine.Object.Instantiate(customNote);
                    fakeMesh.name = name;
                    fakeMesh.transform.SetParent(child);
                    fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                    //FieldInfo field = ____colorManager.GetType().GetField("_colorA", BindingFlags.Instance | BindingFlags.NonPublic);
                    //object leftColor = field.GetValue(____colorManager);
                    //FieldInfo field2 = ____colorManager.GetType().GetField("_colorB", BindingFlags.Instance | BindingFlags.NonPublic);
                    //object rightColor = field2.GetValue(____colorManager);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        internal static void Postfix(NoteController noteController, ref MeshRenderer ____arrowMeshRenderer, ref SpriteRenderer ____arrowGlowSpriteRenderer, ref SpriteRenderer ____circleGlowSpriteRenderer)
        {
            try
            {
                CustomNote activeNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote];
                MeshRenderer noteMesh = noteController.gameObject.GetComponentInChildren<MeshRenderer>();
                Transform child = noteController.gameObject.transform.Find("NoteCube");

                if (activeNote.FileName != "DefaultNotes")
                {
                    GameObject oldCustomNote = child.Find("customNoteArrow")?.gameObject;
                    GameObject oldCustomNoteDot = child.Find("customNoteDot")?.gameObject;

                    // Check and activate/deactivate custom note objects attached to a pooled note object
                    if (noteController.noteData.noteType == NoteType.NoteA ||
                        noteController.noteData.noteType == NoteType.NoteB)
                    {
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
                    noteMesh.enabled = false;

                    if (activeNote.NoteDescriptor.DisableBaseNoteArrows)
                    {
                        ____arrowMeshRenderer.enabled = false;
                        ____arrowGlowSpriteRenderer.enabled = false;
                        ____circleGlowSpriteRenderer.enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }

    [HarmonyPatch(typeof(BombNoteController))]
    [HarmonyPatch("Init", MethodType.Normal)]
    internal class BombInitPatch
    {
        internal static void Prefix(ref BombNoteController __instance)
        {
            try
            {
                MeshRenderer bombMesh = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
                CustomNote activeNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote];
                Transform child = __instance.gameObject.transform.Find("Mesh");

                // Only instantiate a new CustomNote if one is not already attached to this object
                // and we are not using the default ones
                if (activeNote.FileName != "DefaultNotes" &&
                    !child.Find("customNote")?.gameObject)
                {
                    if (activeNote.NoteBomb)
                    {
                        GameObject customBomb = activeNote.NoteBomb;

                        GameObject fakeMesh = UnityEngine.Object.Instantiate(customBomb);
                        fakeMesh.name = "customNote";
                        fakeMesh.transform.SetParent(child);
                        fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        fakeMesh.transform.Rotate(new Vector3(0f, 0f, 90f), Space.Self);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        internal static void Postfix(ref BombNoteController __instance)
        {
            try
            {
                MeshRenderer bombMesh = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
                CustomNote activeNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote];
                Transform child = __instance.gameObject.transform.Find("Mesh");

                if (activeNote.FileName != "DefaultNotes")
                {
                    if (activeNote.NoteBomb)
                    {
                        // Hide certain parts of the default bomb which is not required
                        bombMesh.enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
