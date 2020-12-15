using CustomNotes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomNotes.Utilities
{
    public class LayerUtils
    {
        public static bool HMDOverride = false;
        public static bool CameraSet = false;

        /// <summary>
        /// Note layer type.
        /// </summary>
        public enum NoteLayer : int
        {
            Note = 8,
            FirstPerson = 23,
            ThirdPerson = 8
        }

        /// <summary>
        /// Camera type. Determines culling mask.
        /// </summary>
        public enum CameraView
        {
            Default,
            FirstPerson,
            ThirdPerson
        }

        /// <summary>
        /// Sets the type of a camera to first/third person.
        /// </summary>
        /// <param name="cam">Camera</param>
        /// <param name="view">Type to set the camera to.</param>
        /// <param name="overrideCheck">Unrequired boolean to override the camera cooldown check.</param>
        public static void SetCamera(Camera cam, CameraView view, bool overrideCheck = false)
        {
            if(view == CameraView.FirstPerson)
            {
                cam.cullingMask &= ~(1 << (int)NoteLayer.ThirdPerson);
                cam.cullingMask |= 1 << (int)NoteLayer.FirstPerson;
            }
            else if(view == CameraView.ThirdPerson || view == CameraView.Default)
            {
                cam.cullingMask &= ~(1 << (int)NoteLayer.FirstPerson);
                cam.cullingMask |= 1 << (int)NoteLayer.ThirdPerson;
            }

            if (!overrideCheck)
            {
                CameraSet = true;
            }
        }


        /// <summary>
        /// Recursively sets the layer of a GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="layer">The layer to recursively set.</param>
        public static void SetLayer(GameObject gameObject, NoteLayer layer) => SetLayer(gameObject, (int)layer);

        /// <summary>
        /// Recursively sets the layer of a GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="layer">The layer to recursively set.</param>
        public static void SetLayer(GameObject gameObject, int layer)
        {
            if (gameObject == null) return;
            if (gameObject.layer == layer) return;
            gameObject.layer = layer;
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Recursively sets the layer of a CustomNote.
        /// </summary>
        /// <param name="customNote">The custom note.</param>
        /// <param name="layer">The layer to recursively set.</param>
        public static void SetNoteLayer(CustomNote customNote, NoteLayer layer) => SetNoteLayer(customNote, (int)layer);

        /// <summary>
        /// Recursively sets the layer of a CustomNote.
        /// </summary>
        /// <param name="customNote">The custom note.</param>
        /// <param name="layer">The layer to recursively set.</param>
        public static void SetNoteLayer(CustomNote customNote, int layer)
        {
            Console.WriteLine("LAYER");
            Console.WriteLine(layer);

            SetLayer(customNote.NoteLeft, layer);
            SetLayer(customNote.NoteRight, layer);
            SetLayer(customNote.NoteDotLeft, layer);
            SetLayer(customNote.NoteDotRight, layer);
            if (customNote.NoteBomb != null)
            {
                SetLayer(customNote.NoteBomb, layer);
            }
        }

        /// <summary>
        /// Set the global watermark to show that the player has hmd only custom notes.
        /// </summary>
        public static void SetWatermark()
        {
            // temporarily just a big cube for now. find an actual watermark later.
            GameObject tempWatermark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tempWatermark.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            tempWatermark.transform.position = new Vector3(0, -0.75f, 0);
            SetLayer(tempWatermark, NoteLayer.ThirdPerson);
        }

        /// <summary>
        /// Force custom notes to only be visible in-headset, regardless of the player's settings.
        /// </summary>
        public static void EnableHMDOnly()
        {
            HMDOverride = true;
        }

        /// <summary>
        /// Disabling forcing custom notes to only be visible in-headset.
        /// </summary>
        public static void DisableHMDOnly()
        {
            HMDOverride = false;
        }
    }
}
