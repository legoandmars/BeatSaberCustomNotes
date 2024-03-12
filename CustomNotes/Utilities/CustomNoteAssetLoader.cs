using AssetBundleLoadingTools.Utilities;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace CustomNotes.Utilities
{
    internal class CustomNoteAssetLoader
    {
        public static GameObject LoadNoteObject(AssetBundle bundle)
        {
            GameObject noteObject = bundle.LoadAsset<GameObject>("assets/_customnote.prefab");
            return noteObject;
        }

        public static GameObject LoadNoteWithRepair(AssetBundle bundle, string fileName)
        {
            GameObject noteObject = LoadNoteObject(bundle);
            noteObject = FixNoteShaders(noteObject, fileName);
            return noteObject;
        }

        public static GameObject FixNoteShaders(GameObject noteObject, string fileName)
        {
            Logger.log.Debug($"Repairing shaders for {fileName}");
            try
            {
                List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(noteObject);
                var shaderReplacementInfo = ShaderRepair.FixShadersOnMaterials(materials);

                if (shaderReplacementInfo.AllShadersReplaced == false)
                {
                    Logger.log.Warn("Missing shader replacement data:");
                    foreach (var shaderName in shaderReplacementInfo.MissingShaderNames)
                    {
                        Logger.log.Warn($"\t- {shaderName}");
                    }
                }
                else
                {
                    Logger.log.Debug("All shaders replaced!");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Problem encountered when attempting shader repair for {fileName}");
                Logger.log.Error(ex);
            }
            return noteObject;
        } 
    }
}
