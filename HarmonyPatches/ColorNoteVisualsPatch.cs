using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Collections;
using System.Reflection;
namespace CustomNotes.HarmonyPatches
{

    [HarmonyPatch(typeof(ColorNoteVisuals), "HandleNoteControllerDidInitEvent")]
    internal class ColorNoteVisualsPatch
    {
        public static void Postfix(ref ColorNoteVisuals __instance, NoteController noteController, ref MeshRenderer ____arrowMeshRenderer, ref SpriteRenderer ____arrowGlowSpriteRenderer, ref SpriteRenderer ____circleGlowSpriteRenderer, ref float ____arrowGlowIntensity, ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers, ref int ____colorID, ref ColorManager ____colorManager)
        {
            try
            {
                var noteMesh = noteController.gameObject.GetComponentInChildren<MeshRenderer>();
                //           noteMesh.enabled = true;
                CustomNote activeNote = Plugin.customNotes[Plugin.selectedNote];
                Transform child = noteController.gameObject.transform.Find("NoteCube");
                GameObject.Destroy(child.Find("customNote")?.gameObject);
                if (activeNote.path != "DefaultNotes")
                {
                    GameObject customNote;
                    switch (noteController.noteData.noteType)
                    {
                        case NoteType.NoteA:
                            if (noteController.noteData.cutDirection == NoteCutDirection.Any)
                                customNote = activeNote.noteDotLeft ?? activeNote.noteLeft;
                            else
                                customNote = activeNote.noteLeft;
                            break;
                        case NoteType.NoteB:
                            if (noteController.noteData.cutDirection == NoteCutDirection.Any)
                                customNote = activeNote.noteDotRight ?? activeNote.noteRight;
                            else
                                customNote = activeNote.noteRight;
                            break;
                        default:
                            return;

                    }
                    noteMesh.enabled = false;

                    if (activeNote.noteDescriptor.UsesNoteColor)
                    {
                        Color noteColor = ____colorManager.ColorForNoteType(noteController.noteData.noteType) * activeNote.noteDescriptor.NoteColorStrength;

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
                    if (activeNote.noteDescriptor.DisableBaseNoteArrows)
                    {
                        ____arrowMeshRenderer.enabled = false;
                        ____arrowGlowSpriteRenderer.enabled = false;
                        ____circleGlowSpriteRenderer.enabled = false;
                        //  if (noteController.noteData.cutDirection != NoteCutDirection.Any)
                        //      noteController.gameObject.transform.Find("NoteCube").Find("NoteArrow").GetComponent<MeshRenderer>().enabled = false;
                        //  noteController.gameObject.transform.Find("NoteCube").Find("NoteArrowGlow").GetComponent<SpriteRenderer>().enabled = false;
                        //  noteController.gameObject.transform.Find("NoteCube").Find("NoteCircleGlow").GetComponent<SpriteRenderer>().enabled = false;
                    }

                    GameObject fakeMesh = GameObject.Instantiate(customNote);
                    fakeMesh.name = "customNote";
                    fakeMesh.transform.SetParent(child);
                    fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                    /*var field = ____colorManager.GetType().GetField("_colorA", BindingFlags.Instance | BindingFlags.NonPublic);
                    var leftColor = field.GetValue(____colorManager);
                    var field2 = ____colorManager.GetType().GetField("_colorB", BindingFlags.Instance | BindingFlags.NonPublic);
                    var rightColor = field2.GetValue(____colorManager);
                    */
                }

            }
            catch (Exception err)
            {
                Logger.log.Error(err);
            }
        }
    }

    [HarmonyPatch(typeof(BombNoteController), "Init")]
    class BombInitPatch
    {
        public static void Postfix(ref BombNoteController __instance)
        {
            try
            {

                var bombMesh = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
                bombMesh.enabled = true;
                CustomNote activeNote = Plugin.customNotes[Plugin.selectedNote];
                Transform child = __instance.gameObject.transform.Find("Mesh");
                GameObject.Destroy(child.Find("customNote")?.gameObject);
                if (activeNote.path != "DefaultNotes")
                {
                    GameObject customNote;
                    if (activeNote.noteBomb)
                    {
                        customNote = activeNote.noteBomb;

                    }
                    else
                        return;
                    bombMesh.enabled = false;
                    GameObject fakeMesh = GameObject.Instantiate(customNote);
                    fakeMesh.name = "customNote";
                    fakeMesh.transform.SetParent(child);
                    fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                }



            }
            catch (Exception err)
            {
                Logger.log.Error(err);
            }
        }
    }

}
