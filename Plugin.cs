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
        internal static Material noteMaterial = null;
        internal static ColorManager colorManager;
        public void Init(IPALogger logger)
        {
            Logger.log = logger;
        }

        private AssetBundle assetBundle;

        private NoteDescriptor selectedNoteDescriptor;
        private GameObject noteLeft;
        private GameObject noteRight;

        private List<string> customNotePaths = new List<string>();
        private string selectedNote;

        public void OnApplicationStart()
        {
            Console.WriteLine("Starting CustomNotes!");
            loadCustomNotes();
        }

        private bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }

        public void loadCustomNotes()
        {
            System.IO.Directory.CreateDirectory(Path.Combine(Application.dataPath, "../CustomNotes/"));
            customNotePaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomNotes/"),
                "*.note", SearchOption.TopDirectoryOnly).ToList());
            customNotePaths.Insert(0, "DefaultNotes");
            Console.WriteLine("Found " + customNotePaths.Count + " note(s)");
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
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (customNotePaths.Count == 1) return;
                var oldIndex = customNotePaths.IndexOf(selectedNote);
                if (oldIndex >= customNotePaths.Count - 1)
                {
                    oldIndex = -1;
                }

                var newNote = customNotePaths[oldIndex + 1];
                LoadNoteAsset(newNote);
            }
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {

        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "GameCore")
            {
                Console.WriteLine("Custom Notes - Loading Scene");
                var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
                if (spawnController)
                {
                    spawnController.noteDidStartJumpEvent -= injectNotes;
                    spawnController.noteDidStartJumpEvent += injectNotes;
                }
                if (colorManager == null)
                    colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().First();
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        private void injectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteContoller)
        {
            try
            {
                Transform child = noteContoller.gameObject.transform.Find("NoteCube");
                GameObject.Destroy(child.Find("customNote")?.gameObject);
                if (selectedNote != "DefaultNotes" && noteLeft != null)
                {
                    GameObject customNote;
                    if (noteContoller.noteData.noteType == NoteType.NoteA)
                    {
                            customNote = noteLeft;
                    }
                    else if (noteContoller.noteData.noteType == NoteType.NoteB)
                    {
                            customNote = noteRight;
                    }
                    else return;
                    var noteMesh = noteContoller.gameObject.GetComponentInChildren<MeshRenderer>();
                    noteMesh.enabled = false;
                    GameObject fakeMesh = GameObject.Instantiate(customNote);
                    fakeMesh.name = "customNote";
                    fakeMesh.transform.SetParent(child);
                    fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    fakeMesh.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);

                }


                if (selectedNote != "DefaultNotes")
                {
                    if (noteLeft != null)
                    {
                    }
                }
            }
            catch (Exception err)
            {
                Logger.log.Error(err);
            }
        }

        public void LoadNoteAsset(string path)
        {
         //   Console.WriteLine("LOADING " + path);
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
            }

            if (path != "DefaultNotes")
            {
                noteLeft = null;
                noteRight = null;
          //      Console.WriteLine("STARTING");
                selectedNote = path;
                assetBundle = AssetBundle.LoadFromFile(selectedNote);
           //     var isPath = IsValidPath(selectedNote);
        //        Console.WriteLine("IS PATH: " + isPath);
        /*
                if (isPath == true)
                {
                    assetBundle = AssetBundle.LoadFromFile(selectedNote);
                }
                else if (isPath == false)
                {
                    try
                    {
                        assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(selectedNote));
                    }
                    catch
                    {
                        Logger.log.Warn("Failed to load as resource stream - path does not exist");
                    }
                }
                */
                foreach (string assetName in assetBundle.GetAllAssetNames())
                {
                    Console.WriteLine(assetName);
                }
                if (assetBundle == null)
                {
                    Logger.log.Warn("something went wrong getting the asset bundle!");
                }
                else
                {
                   
                    GameObject note = assetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");
                    selectedNoteDescriptor = note?.GetComponent<NoteDescriptor>();
                    //         Console.WriteLine("Succesfully obtained the asset bundle!");
                    //            foreach (var c in assetBundle.AllAssetNames())
                    //                 Logger.log.Info(c);
                   noteLeft = note.transform.Find("NoteLeft").gameObject;
                    noteRight = note.transform.Find("NoteRight").gameObject;
             //       noteLeft = assetBundle.LoadAsset<GameObject>("assets/blue_block.prefab");
             //       if (noteLeft == null)
             //           noteLeft = assetBundle.LoadAsset<GameObject>("assets/materials/cubemesh.prefab");

//                    noteRight = assetBundle.LoadAsset<GameObject>("assets/red_block.prefab");
                }
            }
            else
            {
                if (assetBundle != null)
                {
                    assetBundle.Unload(true);
                    assetBundle = null;
                    noteLeft = null;
                    noteRight = null;
                }
                selectedNote = "DefaultNotes";
            }
            Logger.log.Info($"Loaded custom note {selectedNote}");
        }


    }
}