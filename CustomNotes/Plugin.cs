using IPA;
using System.IO;
using IPA.Config;
using IPA.Utilities;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using CustomNotes.HarmonyPatches;
using CustomNotes.Installers;
using CustomNotes.Settings.Utilities;
using IPALogger = IPA.Logging.Logger;
using System;

namespace CustomNotes
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string InstanceId = "com.legoandmars.beatsaber.customnotes";
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomNotes");

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.log = logger;
            zenjector.OnApp<CustomNotesCoreInstaller>().WithParameters(config.Generated<PluginConfig>());
            zenjector.OnMenu<CustomNotesMenuInstaller>();
            zenjector.OnGame<CustomNotesGameInstaller>(false).ShortCircuitForTutorial();
        }

        [OnEnable]
        public void OnEnable()
        {
            try
            {
                CustomNotesPatches.ApplyHarmonyPatches();
            }
            catch
            {
                Logger.log.Warn("Camera Plus not detected, disabling CameraPlus harmony patch.");
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            CustomNotesPatches.RemoveHarmonyPatches();
        }
    }
}