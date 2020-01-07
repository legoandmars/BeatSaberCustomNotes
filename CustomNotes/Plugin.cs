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
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace CustomNotes
{
    public class Plugin : IBeatSaberPlugin, IDisablablePlugin
    {
        public static string PluginName => "CustomNotes";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default
        public static string PluginAssetPath => Path.Combine(BeatSaber.InstallPath, "CustomNotes");

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider, PluginLoader.PluginMetadata metadata)
        {
            Logger.log = logger;
            Configuration.Init(cfgProvider);

            if (metadata?.Version != null)
            {
                PluginVersion = metadata.Version;
            }
        }

        public void OnEnable() => Load();
        public void OnDisable() => Unload();
        public void OnApplicationQuit() => Unload();

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                int noteCount = NoteAssetLoader.CustomNoteObjects.Count;
                if (noteCount != 1)
                {
                    if (NoteAssetLoader.SelectedNote >= noteCount - 1)
                    {
                        NoteAssetLoader.SelectedNote = -1;
                    }

                    NoteAssetLoader.SelectedNote++;
                    string noteName = NoteAssetLoader.CustomNoteObjects[NoteAssetLoader.SelectedNote].NoteDescriptor.NoteName;

                    Logger.log.Info($"Switched to note '{noteName}'");
                    CheckCustomNotesScoreDisable();
                }
            }
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
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
        }

        public void OnApplicationStart() { }
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnFixedUpdate() { }

        private void Load()
        {
            Configuration.Load();
            NoteAssetLoader.LoadCustomNotes();
            Patches.ApplyHarmonyPatches();
            SettingsUI.CreateMenu();

            Logger.log.Info($"{PluginName} v.{PluginVersion} has started.");
        }

        private void Unload()
        {
            Patches.RemoveHarmonyPatches();
            ScoreUtility.Cleanup();

            Configuration.Save();
        }

        private void CheckCustomNotesScoreDisable()
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
