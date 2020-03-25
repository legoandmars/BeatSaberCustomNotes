using CustomNotes.Data;
using CustomNotes.HarmonyPatches;
using CustomNotes.Settings;
using CustomNotes.Settings.UI;
using CustomNotes.Utilities;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using System.IO;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace CustomNotes
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string PluginName => "CustomNotes";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomNotes");

        [Init]
        public void Init(IPALogger logger, Config config, PluginMetadata metadata)
        {
            Logger.log = logger;
            Configuration.Init(config);

            if (metadata?.Version != null)
            {
                PluginVersion = metadata.Version;
            }
        }

        [OnEnable]
        public void OnEnable() => Load();
        [OnDisable]
        public void OnDisable() => Unload();

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
            Configuration.Load();
            NoteAssetLoader.Load();
            CustomNotesPatches.ApplyHarmonyPatches();
            SettingsUI.CreateMenu();
            AddEvents();

            Logger.log.Info($"{PluginName} v.{PluginVersion} has started.");
        }

        private void Unload()
        {
            RemoveEvents();
            CustomNotesPatches.RemoveHarmonyPatches();
            ScoreUtility.Cleanup();
            Configuration.Save();
            NoteAssetLoader.Clear();
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
