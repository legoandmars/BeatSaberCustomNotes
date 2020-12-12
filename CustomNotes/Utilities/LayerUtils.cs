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
        /// <summary>
        /// Note layer type.
        /// </summary>
        public enum NoteLayer : int
        {
            Note = 8,
            FirstPerson = 23,
            ThirdPerson = 25
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
            if (gameObject.layer == layer) return;
            gameObject.layer = layer;
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = layer;
            }
        }
    }
}
