using IPA;
using System.IO;
using IPA.Loader;
using IPA.Config;
using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Zenject;
using CustomNotes.Data;
using IPA.Config.Stores;
using System.Reflection;
using CustomNotes.Utilities;
using CustomNotes.Installers;
using CustomNotes.Settings.UI;
using UnityEngine.SceneManagement;
using CustomNotes.Settings.Utilities;
using IPALogger = IPA.Logging.Logger;

namespace CustomNotes
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string PluginName => "CustomNotes";
        public const string InstanceId = "com.legoandmars.beatsaber.customnotes";
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomNotes");

        private readonly Harmony _harmony;

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.log = logger;
            _harmony = new Harmony(InstanceId);
            zenjector.OnApp<CustomNotesCoreInstaller>().WithParameters(config.Generated<PluginConfig>());
        }

        [OnEnable]
        public void OnEnable()
        {
            NoteAssetLoader.Load();
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            NoteAssetLoader.Clear();
            _harmony.UnpatchAll(InstanceId);
        }

        public void OnGameSceneLoaded()
        {
            CustomNote activeNote = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote];
            if (activeNote.FileName != "DefaultNotes")
            {
                MaterialSwapper.GetMaterials();
                MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteRight);

                if (activeNote.NoteDotLeft != null)
                {
                    MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteDotLeft);
                }

                if (activeNote.NoteDotRight != null)
                {
                    MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteDotRight);
                }

                if (activeNote.NoteBomb != null)
                {
                    MaterialSwapper.ReplaceMaterialsForGameObject(activeNote.NoteBomb);
                }

                CheckCustomNotesScoreDisable();
            }
            else if (ScoreUtility.ScoreIsBlocked)
            {
                ScoreUtility.EnableScoreSubmission("ModifiersEnabled");
            }
        }

        private void Load()
        {
            NoteAssetLoader.Load();
            SettingsUI.CreateMenu();
            AddEvents();
        }

        private void Unload()
        {
            RemoveEvents();
            ScoreUtility.Cleanup();
            SettingsUI.RemoveMenu();
        }

        private void AddEvents()
        {
            RemoveEvents();
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += OnGameSceneLoaded;
        }

        private void RemoveEvents()
        {
            BS_Utils.Utilities.BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
        }

        internal static void CheckCustomNotesScoreDisable()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
            {
                string fileName = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote].FileName;
                if (fileName != "DefaultNotes")
                {
                    if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.ghostNotes == true
                        || BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows == true)
                    {
                        ScoreUtility.DisableScoreSubmission("ModifiersEnabled");
                    }
                    else
                    {
                        ScoreUtility.EnableScoreSubmission("ModifiersEnabled");
                    }
                }
            }
        }
    }
}
