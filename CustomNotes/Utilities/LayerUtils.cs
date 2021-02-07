using CustomNotes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage;
using TMPro;
using HMUI;
using UnityEngine.UI;
using CustomNotes.Settings.Utilities;

namespace CustomNotes.Utilities
{
    public class LayerUtils
    {
        public static bool HMDOverride = false;
        public static PluginConfig pluginConfig;

        private static GameObject _watermarkObject;

        public static bool HMDOnly
        {
            get
            {
                return (pluginConfig?.HMDOnly == false && HMDOverride == false) ? false : true;
            }
        }

        /// <summary>
        /// Note layer type.
        /// </summary>
        public enum NoteLayer : int
        {
            Note = 8,
            FirstPerson = 24,
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
        public static void SetCamera(Camera cam, CameraView view)
        {
            if (cam == null) return;
            switch(view)
            {
                default:
                case CameraView.Default:
                case CameraView.ThirdPerson:
                    cam.cullingMask &= ~(1 << (int) NoteLayer.FirstPerson);
                    cam.cullingMask |= 1 << (int) NoteLayer.ThirdPerson;
                    break;
                case CameraView.FirstPerson:
                    cam.cullingMask &= ~(1 << (int) NoteLayer.ThirdPerson);
                    cam.cullingMask |= 1 << (int) NoteLayer.FirstPerson;
                    break;
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
        /// Set the global watermark to show that the player has hmd only custom notes.
        /// </summary>
        public static void CreateWatermark()
        {
            if (_watermarkObject != null) return;
            _watermarkObject = new GameObject("CustomNotes Watermark");
            _watermarkObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            _watermarkObject.transform.position = new Vector3(0f, 0.025f, 0.8f);
            _watermarkObject.transform.rotation = Quaternion.Euler(90, 0, 0);

            Canvas watermarkCanvas = _watermarkObject.AddComponent<Canvas>();
            watermarkCanvas.renderMode = RenderMode.WorldSpace;
            ((RectTransform)watermarkCanvas.transform).sizeDelta = new Vector2(100, 50);

            CurvedCanvasSettings canvasSettings = _watermarkObject.AddComponent<CurvedCanvasSettings>();
            canvasSettings.SetRadius(0f);

            CurvedTextMeshPro text = (CurvedTextMeshPro)BeatSaberUI.CreateText((RectTransform)watermarkCanvas.transform, "Custom Notes Enabled", new Vector2(0, 0));
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.95f, 0.95f, 0.95f);

            SetLayer(_watermarkObject, NoteLayer.ThirdPerson);
        }

        public static void DestroyWatermark()
        {
            if (_watermarkObject == null) return;
            _watermarkObject.SetActive(false);
            UnityEngine.Object.Destroy(_watermarkObject);
            _watermarkObject = null;
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

        /* // Game Version 1.13.2
        8  : "Note"
        9  : "NoteDebris"
        10 : "Avatar"
        11 : "Obstacle"
        12 : "Saber"
        13 : "NeonLight"
        14 : "Environment"
        15 : "GrabPassTexture1"
        16 : "CutEffectParticles"
        17 : ""
        18 : ""
        19 : "NonReflectedParticles"
        20 : ""
        21 : ""
        22 : ""
        23 : ""
        24 : ""
        25 : "FixMRAlpha"
        26 : ""
        27 : "DontShowInExternalMRCamera"
        28 : "PlayersPlace"
        29 : "Skybox"
        30 : ""
        31 : "Reserved"
        */

        /// <summary>
        /// Prints all layer names with their ids
        /// </summary>
        public static void PrintLayerNames()
        {
            for (int i = 8; i < 32; i++)
            {
                Logger.log.Notice($"LayerID:{i} : \"{LayerMask.LayerToName(i)}\"");
            }
        }
    }
}
