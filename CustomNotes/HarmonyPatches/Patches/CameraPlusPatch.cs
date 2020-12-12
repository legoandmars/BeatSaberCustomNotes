using CameraPlus;
using CustomNotes.Utilities;
using HarmonyLib;
using UnityEngine;

namespace CustomNotes.HarmonyPatches
{
    [HarmonyPatch(typeof(CameraPlusBehaviour))]
    [HarmonyPatch("SetCullingMask", MethodType.Normal)]
    internal class CameraPlusPatch
    {
        private static void Postfix(ref Camera ____cam)
        {
            LayerUtils.SetCamera(____cam, LayerUtils.CameraView.ThirdPerson, true);
        }
    }
}
