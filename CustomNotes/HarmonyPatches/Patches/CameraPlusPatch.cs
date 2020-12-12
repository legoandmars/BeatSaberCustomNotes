using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using CameraPlus;
using UnityEngine;
using Zenject;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;

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
