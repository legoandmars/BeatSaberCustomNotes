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
        private GameObject cubeMeshRed;
        private List<string> customNotePaths;
        private string selectedNote;
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
                "*.note", SearchOption.AllDirectories).ToList());
            Console.WriteLine("Found " + customNotePaths.Count + " note(s)");
            customNotePaths.Insert(0, "DefaultNotes");
            customNotePaths.Insert(0, "CustomNotes.minecraft.note");
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
            if (scene.name == "GameCore")
            {
                Console.WriteLine("Custom Notes - Loading Scene");
                var spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault<BeatmapObjectSpawnController>();
                if (spawnController)
                {
                    spawnController.noteDidStartJumpEvent -= injectNotes;
                    spawnController.noteDidStartJumpEvent += injectNotes;
                }
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        private void injectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteContoller)
        {
            try
            {
                if (noteContoller.gameObject.GetComponentInChildren<Renderer>() != null)
                {
                    if (noteContoller.gameObject.GetComponentInChildren<Renderer>().bounds != null)
                    {
                        var boundsString = noteContoller.gameObject.GetComponentInChildren<Renderer>().bounds;//.ToString();
                        if (boundsString != null)
                        {
                            //Console.WriteLine(boundsString.ToString());
                        }
                    }
                }
                foreach (Transform child in noteContoller.gameObject.transform)
                {
                    if (!child.Find("fakeMesh") && selectedNote != "DefaultNotes" && cubeMesh != null)
                    {
                        GameObject whichMesh;
                        if (noteContoller.noteData.noteType == NoteType.NoteA)
                        {
                            whichMesh = cubeMeshRed;
                        }
                        else if (noteContoller.noteData.noteType == NoteType.NoteB)
                        {
                            whichMesh = cubeMesh;
                        }
                        else return;
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
                        //meshRender.material.SetTexture("_Tex", cubeTexture);
                        Console.WriteLine(meshRender.material.name);
                        //Console.WriteLine(meshRender.material.mainTexture.name);
                        Console.WriteLine(meshRender.material.shader.name);

                        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.SetParent(child);
                        cube.transform.localPosition = new Vector3(0, 0, 0);
                        cube.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        cube.GetComponent<Renderer>().material = child.GetComponentInChildren<MeshRenderer>().material;
                        cube.name = "fakeMesh";*/
                    }
                }

                if (selectedNote != "DefaultNotes")
                {
                    if (cubeMesh != null)
                    {
                        //Console.WriteLine(noteContoller.gameObject.GetComponentInChildren<MeshFilter>().mesh.bounds);
                        //Console.WriteLine(cubeMesh.name);
                        /*noteContoller.gameObject.GetComponentInChildren<MeshFilter>().mesh = cubeMesh.GetComponent<MeshFilter>().mesh;
                        noteContoller.gameObject.GetComponentInChildren<Renderer>().material = cubeMaterial;
                        */
                        //noteContoller.gameObject.GetComponentInChildren<Renderer>().material.shader = Shader.Find("BeatSaber/Unlit Glow");
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
                var isPath = IsValidPath(selectedNote);
                Console.WriteLine("IS PATH: " + isPath);
                if (isPath == true)
                {
                    assetBundle = AssetBundle.LoadFromFile(selectedNote);
                }else if(isPath == false){
                    try
                    {
                        assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(selectedNote));
                    }
                    catch
                    {
                        Console.WriteLine("Failed to load as resource stream - path does not exist");
                    }
                }
                foreach (string assetName in assetBundle.GetAllAssetNames())
                {
                    Console.WriteLine(assetName);
                }
                if (assetBundle == null)
                {
                    Console.WriteLine("something went wrong getting the asset bundle!");
                }
                else
                {
                    Console.WriteLine("Succesfully obtained the asset bundle!");
                    //SaberScript.CustomSaber = assetBundle;
                    cubeMesh = assetBundle.LoadAsset<GameObject>("assets/blue_block.prefab");
                    cubeMeshRed = assetBundle.LoadAsset<GameObject>("assets/red_block.prefab");
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
                    cubeMeshRed = null;
                }
                selectedNote = "DefaultNotes";
            }

            //PlayerPrefs.SetString("lastSaber", _currentSaberPath);
            Console.WriteLine($"Loaded custom note {path}");
        }


    }
}
