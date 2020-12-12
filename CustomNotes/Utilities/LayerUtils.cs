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
        public static bool CameraSet = false;

        /// <summary>
        /// Note layer type.
        /// </summary>
        public enum NoteLayer : int
        {
            Note = 8,
            FirstPerson = 23
        }

        public static void SetCamera()
        {
            if(CameraSet == false)
            {
                Logger.log.Info("I am setting the culling mask");
                Camera.main.cullingMask &= ~(1 << (int)NoteLayer.Note);
                Camera.main.cullingMask |= 1 << (int)NoteLayer.FirstPerson;
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
    }
}
