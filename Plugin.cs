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
using CustomUI.BeatSaber;
using HMUI;
namespace CustomNotes
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static CustomMenu _notesMenu;
        internal static Material noteMaterial = null;
        internal static ColorManager colorManager;
        public void Init(IPALogger logger)
        {
            Logger.log = logger;
        }
        internal static CustomNote[] customNotes;
        internal static int selectedNote = 0;
        private List<string> customNotePaths = new List<string>();

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
            Logger.log.Info("Found " + customNotePaths.Count + " note(s)");
            List<CustomNote> loadedNotes = new List<CustomNote>();
            for (int i = 0; i < customNotePaths.Count; i++)
            {
                loadedNotes.Add(new CustomNote(customNotePaths[i]));
            }
            customNotes = loadedNotes.ToArray();
            //       LoadNoteAsset(customNotePaths[0]);
        }

        public void OnApplicationQuit()
        {
            //    Logger.log.Debug("OnApplicationQuit");
        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (customNotePaths.Count == 1) return;
                if (selectedNote >= customNotes.Length - 1)
                {
                    selectedNote = -1;
                }
                selectedNote++;
                Logger.log.Info("Switched To Note:" + customNotes[selectedNote].noteDescriptor.NoteName);
                CheckCustomNotesScoreDisable();
                //         LoadNoteAsset(newNote);
            }
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
            {
                //      Console.WriteLine("Custom Notes - Loading Scene");
                var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
                if (spawnController)
                {
                    spawnController.noteDidStartJumpEvent -= injectNotes;
                    spawnController.noteDidStartJumpEvent += injectNotes;
                }
                if (colorManager == null)
                    colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().First();
                CheckCustomNotesScoreDisable();
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
            {
                if (_notesMenu == null)
                {
                    _notesMenu = BeatSaberUI.CreateCustomMenu<CustomMenu>("Custom Notes");
                    UI.NoteListViewController noteListViewController = BeatSaberUI.CreateViewController<UI.NoteListViewController>();
                    noteListViewController.backButtonPressed += delegate () { _notesMenu.Dismiss(); };
                    _notesMenu.SetMainViewController(noteListViewController, true);
                    noteListViewController.DidSelectRowEvent += delegate (TableView view, int row) { selectedNote = row; };
                }

                CustomUI.MenuButton.MenuButtonUI.AddButton("CustomNotes", delegate () { _notesMenu.Present(); });
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        private void injectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteContoller)
        {
            try
            {
                var noteMesh = noteContoller.gameObject.GetComponentInChildren<MeshRenderer>();
                noteMesh.enabled = true;
                if (noteContoller.noteData.noteType != NoteType.Bomb)
                {
                    Transform child = noteContoller.gameObject.transform.Find("NoteCube");
                    GameObject.Destroy(child.Find("customNote")?.gameObject);
                    if (customNotes[selectedNote].path != "DefaultNotes")
                    {
                        GameObject customNote;
                        switch (noteContoller.noteData.noteType)
                        {
                            case NoteType.NoteA:
                                if (noteContoller.noteData.cutDirection == NoteCutDirection.Any)
                                    customNote = customNotes[selectedNote].noteDotLeft ?? customNotes[selectedNote].noteLeft;
                                else
                                    customNote = customNotes[selectedNote].noteLeft;
                                break;
                            case NoteType.NoteB:
                                if (noteContoller.noteData.cutDirection == NoteCutDirection.Any)
                                    customNote = customNotes[selectedNote].noteDotRight ?? customNotes[selectedNote].noteRight;
                                else
                                    customNote = customNotes[selectedNote].noteRight;
                                break;
                            default:
                                return;

                        }
                        noteMesh.enabled = false;
                        if (customNotes[selectedNote].noteDescriptor.DisableBaseNoteArrows)
                        {
                            if (noteContoller.noteData.cutDirection != NoteCutDirection.Any)
                                noteContoller.gameObject.transform.Find("NoteCube").Find("NoteArrow").GetComponent<MeshRenderer>().enabled = false;
                            noteContoller.gameObject.transform.Find("NoteCube").Find("NoteArrowGlow").GetComponent<SpriteRenderer>().enabled = false;
                            noteContoller.gameObject.transform.Find("NoteCube").Find("NoteCircleGlow").GetComponent<SpriteRenderer>().enabled = false;
                        }

                        GameObject fakeMesh = GameObject.Instantiate(customNote);
                        fakeMesh.name = "customNote";
                        fakeMesh.transform.SetParent(child);
                        fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                    }
                }
                else
                {
                    Transform child = noteContoller.gameObject.transform.Find("Mesh");
                    GameObject.Destroy(child.Find("customNote")?.gameObject);
                    if (customNotes[selectedNote].path != "DefaultNotes")
                    {
                        GameObject customNote;
                        if (customNotes[selectedNote].noteBomb)
                        {
                            customNote = customNotes[selectedNote].noteBomb;

                        }
                        else
                            return;
                        noteMesh.enabled = false;
                        GameObject fakeMesh = GameObject.Instantiate(customNote);
                        fakeMesh.name = "customNote";
                        fakeMesh.transform.SetParent(child);
                        fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
                    }


                }
            }
            catch (Exception err)
            {
                Logger.log.Error(err);
            }
        }
        /*
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
        */
        public void CheckCustomNotesScoreDisable()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
            {
                if (customNotes[selectedNote].path != "DefaultNotes")
                    if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.ghostNotes == true || BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows == true)
                    {
                        BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("Custom Notes");
                        Logger.log.Notice("Disabling Score Submission for GN/DA and Custom Notes");
                    }
            }
        }
    }
}