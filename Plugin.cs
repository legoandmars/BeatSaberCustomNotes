using CustomNotes.ConfigUtilities;
using CustomNotes.HarmonyPatches;
using CustomNotes.UI;
using CustomNotes.Utilities;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes
{
    public class Plugin : IBeatSaberPlugin, IDisablablePlugin
    {
        public static string PluginName => "CustomNotes";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default
        public static string PluginAssetPath => Path.Combine(BeatSaber.InstallPath, "CustomNotes");

        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        internal static ColorManager ColorManager { get; set; }

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider, PluginLoader.PluginMetadata metadata)
        {
            Logger.log = logger;

            configProvider = cfgProvider;
            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig || v.Value == null && v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                }
                config = v;
            });

            PluginVersion = metadata?.Version;
        }

        public void OnApplicationStart() => Load();
        public void OnApplicationQuit() => Unload();
        public void OnEnable() => Load("enabled");
        public void OnDisable() => Unload();

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (NoteAssetLoader.customNoteFiles.Count != 1)
                {
                    if (NoteAssetLoader.selectedNote >= NoteAssetLoader.customNotes.Length - 1)
                    {
                        NoteAssetLoader.selectedNote = -1;
                    }

                    NoteAssetLoader.selectedNote++;
                    Logger.Log($"Switched To Note: {NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote].NoteDescriptor.NoteName}");
                    CheckCustomNotesScoreDisable();
                }
            }
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
            {
                CustomNote activeNote = NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote];
                if (activeNote.NoteDescriptor.NoteName != "Default")
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
                else
                {
                    ScoreUtility.EnableScoreSubmission("ModifiersEnabled");
                }

                if (ColorManager == null)
                {
                    ColorManager = Resources.FindObjectsOfTypeAll<ColorManager>().First();
                }
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
            {
            //    SettingsUI.CreateMenu();
            }
        }

        public void OnSceneUnloaded(Scene scene) { }
        public void OnFixedUpdate() { }

        private void Load(string msg = "started")
        {
            Configuration.Load();

            NoteAssetLoader.LoadCustomNotes();
            Patches.ApplyHarmonyPatches();
            SettingsUI.CreateMenu();
            Logger.Log($"{PluginName} v.{PluginVersion} has been {msg}.", LogLevel.Notice);
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
                if (NoteAssetLoader.customNotes[NoteAssetLoader.selectedNote].FileName != "DefaultNotes")
                {
                    if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.ghostNotes == true ||
                        BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows == true)
                    {
                        ScoreUtility.DisableScoreSubmission("ModifiersEnabled");
                    }
                }
            }
        }
    }
}
