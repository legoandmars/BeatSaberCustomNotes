using CustomNotes.Utilities;
using HarmonyLib;
using IPA.Loader;
using System;
using System.Reflection;
using UnityEngine;

namespace CustomNotes.HarmonyPatches
{
    [HarmonyPatch]
    internal class CameraPlusPatch
    {
        private static MethodBase TargetMethod()
        {
            PluginMetadata cameraPlus = PluginManager.GetPluginFromId("CameraPlus");
            // Patch should only trigger if CameraPlus is installed and a pre-layerfix version
            if (cameraPlus != null && cameraPlus.Assembly.GetName().Version <= new Version("4.7.2")) { 
                return cameraPlus.Assembly.GetType("CameraPlus.CameraPlusBehaviour").GetMethod("SetCullingMask", BindingFlags.Instance | BindingFlags.NonPublic);
            } else return null;
        }

        private static void Postfix(ref Camera ____cam)
        {
            LayerUtils.SetCamera(____cam, LayerUtils.CameraView.ThirdPerson);
        }
    }
}
