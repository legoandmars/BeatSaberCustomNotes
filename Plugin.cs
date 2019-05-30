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

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider) {
            Logger.log = logger;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) => {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }
        private AssetBundle assetBundle;

        private GameObject cubeMesh;
        private GameObject cubeMeshRed;

        private List<string> customNotePaths;
        private string selectedNote;

        public void OnApplicationStart() {
            Console.WriteLine("Starting CustomNotes!");
            loadCustomNotes();
        }

        private bool IsValidPath(string path, bool allowRelativePaths = false) {
            bool isValid = true;

            try {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths) {
                    isValid = Path.IsPathRooted(path);
                } else {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            } catch (Exception ex) {
                isValid = false;
            }

            return isValid;
        }

        public void loadCustomNotes() {
            System.IO.Directory.CreateDirectory(Path.Combine(Application.dataPath, "../CustomNotes/"));
            customNotePaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomNotes/"),
                "*.note", SearchOption.AllDirectories).ToList());
            Console.WriteLine("Found " + customNotePaths.Count + " note(s)");
            customNotePaths.Insert(0, "DefaultNotes");
            customNotePaths.Insert(0, "CustomNotes.minecraft.note");
            LoadNoteAsset(customNotePaths[0]);
        }

        public void OnApplicationQuit() {
            Logger.log.Debug("OnApplicationQuit");
        }

        public void OnFixedUpdate() {

        }

        public void OnUpdate() {
            if (Input.GetKeyDown(KeyCode.N)) {
                if (customNotePaths.Count == 1) return;
                var oldIndex = customNotePaths.IndexOf(selectedNote);
                if (oldIndex >= customNotePaths.Count - 1) {
                    oldIndex = -1;
                }

                var newNote = customNotePaths[oldIndex + 1];
                LoadNoteAsset(newNote);
            }
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) {

        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
            if (scene.name == "GameCore") {
                Console.WriteLine("Custom Notes - Loading Scene");
                var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
                if (spawnController) {
                    spawnController.noteDidStartJumpEvent -= injectNotes;
                    spawnController.noteDidStartJumpEvent += injectNotes;
                }
            }
        }

        public void OnSceneUnloaded(Scene scene) {

        }

        private void injectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteContoller) {
            try {
                foreach (Transform child in noteContoller.gameObject.transform) {
                    if (!child.Find("fakeMesh") && selectedNote != "DefaultNotes" && cubeMesh != null) {
                        GameObject whichMesh;
                        if (noteContoller.noteData.noteType == NoteType.NoteA) {
                            whichMesh = cubeMeshRed;
                        } else if (noteContoller.noteData.noteType == NoteType.NoteB) {
                            whichMesh = cubeMesh;
                        } else return;
                        noteContoller.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                        GameObject fakeMesh = new GameObject("fakeMesh");
                        fakeMesh.transform.SetParent(child);
                        MeshRenderer meshRender = fakeMesh.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
                        MeshFilter meshFilter = fakeMesh.AddComponent(typeof(MeshFilter)) as MeshFilter;
                        fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        fakeMesh.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                        meshFilter.mesh = whichMesh.GetComponent<MeshFilter>().mesh;
                        meshRender.material = whichMesh.GetComponent<Renderer>().material;
                        Console.WriteLine(meshRender.material.name);
                        Console.WriteLine(meshRender.material.shader.name);
                    }
                }

                if (selectedNote != "DefaultNotes") {
                    if (cubeMesh != null) {
                    }
                }
            } catch (Exception err) {
                Console.WriteLine(err);
            }
        }

        public void LoadNoteAsset(string path) {
            Console.WriteLine("LOADING " + path);
            if (assetBundle != null) {
                assetBundle.Unload(true);
            }

            if (path != "DefaultNotes") {
                Console.WriteLine("STARTING");
                selectedNote = path;
                var isPath = IsValidPath(selectedNote);
                Console.WriteLine("IS PATH: " + isPath);
                if (isPath == true) {
                    assetBundle = AssetBundle.LoadFromFile(selectedNote);
                } else if (isPath == false) {
                    try {
                        assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(selectedNote));
                    } catch {
                        Console.WriteLine("Failed to load as resource stream - path does not exist");
                    }
                }
                foreach (string assetName in assetBundle.GetAllAssetNames()) {
                    Console.WriteLine(assetName);
                }
                if (assetBundle == null) {
                    Console.WriteLine("something went wrong getting the asset bundle!");
                } else {
                    Console.WriteLine("Succesfully obtained the asset bundle!");
                    cubeMesh = assetBundle.LoadAsset<GameObject>("assets/blue_block.prefab");
                    cubeMeshRed = assetBundle.LoadAsset<GameObject>("assets/red_block.prefab");
                }
            } else if (path == "DefaultNotes") {
                if (assetBundle != null) {
                    assetBundle.Unload(true);
                    assetBundle = null;
                    cubeMesh = null;
                    cubeMeshRed = null;
                }
                selectedNote = "DefaultNotes";
            }
            Console.WriteLine($"Loaded custom note {path}");
        }


    }
}