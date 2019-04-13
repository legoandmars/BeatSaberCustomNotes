using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace CustomNotes
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }
        private AssetBundle assetBundle;
        private GameObject cubeMesh;
        private List<string> customNotePaths;
        private string selectedNote;
        //private Material cubeMaterial; 
        public void OnApplicationStart()
        {
            Console.WriteLine("Starting CustomNotes!");
            //get embedded assetbundle v
            //assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomNotes.cubebundle"));
            //get mesh FROM said embedded assetbundle v
            //cubeMesh = assetBundle.LoadAsset<GameObject>("assets/materials/cubemesh.prefab");
            //cubeMaterial = assetBundle.LoadAsset<Material>("CubeSkin");
            //Logger.log.Debug("OnApplicationStart");
            loadCustomNotes();
        }

        public void loadCustomNotes()
        {
            customNotePaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomNotes/"),
                "*.note", SearchOption.AllDirectories).ToList());
            Console.WriteLine("Found " + customNotePaths.Count + " note(s)");
            customNotePaths.Insert(0, "DefaultNotes");
            LoadNoteAsset(customNotePaths[0]);
        }

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {
            //if (assetBundle == null) return;
            if (Input.GetKeyDown(KeyCode.N))
            {
                //RetrieveCustomSabers();
                if (customNotePaths.Count == 1) return;
                var oldIndex = customNotePaths.IndexOf(selectedNote);
                if (oldIndex >= customNotePaths.Count - 1)
                {
                    oldIndex = -1;
                }

                var newNote = customNotePaths[oldIndex + 1];
                LoadNoteAsset(newNote);
                /*if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();*/
            }
            /*else if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftAlt))
            {
                RetrieveCustomSabers();
                if (_saberPaths.Count == 1) return;
                var oldIndex = _saberPaths.IndexOf(_currentSaberPath);
                if (oldIndex <= 0)
                {
                    oldIndex = _saberPaths.Count - 1;
                }

                var newSaber = _saberPaths[oldIndex - 1];
                LoadNewSaber(newSaber);
                if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();
            }*/
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {

        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            Console.WriteLine("Custom Notes - Loading Scene");
            var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
            if (spawnController)
            {
                spawnController.noteDidStartJumpEvent += new Action<BeatmapObjectSpawnController, NoteController>(this.injectNotes);
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        private void injectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteContoller)
        {
            try
            {
                if (selectedNote != "DefaultNotes")
                {
                    if (cubeMesh != null)
                    {
                        Console.WriteLine(cubeMesh.name);
                        noteContoller.gameObject.GetComponentInChildren<MeshFilter>().mesh = cubeMesh.GetComponent<MeshFilter>().mesh;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
        /*private void reassignFunction()
        {
            Console.WriteLine("Custom Notes - Reassigning functions");
            var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
            if (spawnController)
            {
                spawnController.noteDidStartJumpEvent -= new Action<BeatmapObjectSpawnController, NoteController>(this.onNoteJump);
                spawnController.noteDidStartJumpEvent += new Action<BeatmapObjectSpawnController, NoteController>(this.onNoteJump);
            }
        }*/

        public void LoadNoteAsset(string path)
        {
            Console.WriteLine("LOADING " + path);
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
            }

            if (path != "DefaultNotes")
            {
                Console.WriteLine("STARTING");
                selectedNote = path;

                assetBundle =
                    AssetBundle.LoadFromFile(selectedNote);
                //Console.WriteLine(assetBundle.GetAllAssetNames()[0]);
                if (assetBundle == null)
                {
                    Console.WriteLine("something went wrong getting the asset bundle");
                }
                else
                {
                    Console.WriteLine("Succesfully obtained the asset bundle!");
                    //SaberScript.CustomSaber = assetBundle;
                    cubeMesh = assetBundle.LoadAsset<GameObject>("assets/materials/cubemesh.prefab");
                    //now we need to unassign the assigned function
                    //reassignFunction();
                }
            }
            else if (path == "DefaultNotes")
            {
                if (assetBundle != null)
                {
                    assetBundle.Unload(true);
                    assetBundle = null;
                    cubeMesh = null;
                }
                selectedNote = "DefaultNotes";
            }

            //PlayerPrefs.SetString("lastSaber", _currentSaberPath);
            Console.WriteLine($"Loaded saber {path}");
        }


    }
}
