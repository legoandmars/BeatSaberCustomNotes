using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using IPA.Loader;
using IPA.Utilities;
using System;
using CustomNotes.Settings.Utilities;

namespace CustomNotes.Utilities
{
    public class Utils
    {
        /// <summary>
        /// Sets the active state of a GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="setActive">Desired state.</param>
        public static bool ActivateGameObject(GameObject gameObject, bool setActive) => ActivateGameObject(gameObject, setActive, null);

        /// <summary>
        /// Sets the active state of a primary GameObject and then sets the opposite active state for the secondary GameObject.
        /// </summary>
        /// <param name="primaryObject">Primary GameObject.</param>
        /// <param name="setActive">Desired state.</param>
        /// <param name="secondaryObject">Secondary GameObject.</param>
        public static bool ActivateGameObject(GameObject primaryObject, bool setActive, GameObject secondaryObject)
        {
            if (primaryObject)
            {
                if (primaryObject.activeSelf != setActive)
                {
                    primaryObject.SetActive(setActive);
                }

                if (secondaryObject != null)
                {
                    return ActivateGameObject(secondaryObject, !setActive);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Colorize a Note based on a ColorManager and CustomNote configuration
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="colorStrength">Color strength</param>
        /// <param name="noteObject">Note to colorize</param>
        public static void ColorizeCustomNote(Color color, float colorStrength, GameObject noteObject)
        {
            if (!noteObject || color == null)
            {
                return;
            }

            Color noteColor = color * colorStrength;

            IEnumerable<Transform> childTransforms = noteObject.GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms)
            {
                DisableNoteColorOnGameobject colorDisabled = childTransform.GetComponent<DisableNoteColorOnGameobject>();
                if (!colorDisabled)
                {
                    Renderer childRenderer = childTransform.GetComponent<Renderer>();
                    if (childRenderer)
                    {
                        childRenderer.material.SetColor("_Color", noteColor);
                    }
                }
            }
            MaterialPropertyBlockController materialPropertyBlockController = noteObject.GetComponent<MaterialPropertyBlockController>(); // Set the color of material property block controllers, for the replaced note shader
            if (materialPropertyBlockController != null)
            {
                materialPropertyBlockController.materialPropertyBlock.SetColor("_Color", noteColor);
                materialPropertyBlockController.ApplyChanges();
            }
        }

        /// <summary>
        /// Adds a MaterialPropertyBlockController to the root of the gameObject. Only selects renderers with specific shaders.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void AddMaterialPropertyBlockController(GameObject gameObject)
        {
            List<Renderer> rendererList = new List<Renderer>();
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer.material.shader.name.ToLower() == "custom/notehd") // only get the replaced note shader
                {
                    rendererList.Add(renderer);
                }
            }
            if (rendererList.Count > 0)
            {
                MaterialPropertyBlockController newController = gameObject.AddComponent<MaterialPropertyBlockController>();
                ReflectionUtil.SetField<MaterialPropertyBlockController, Renderer[]>(newController, "_renderers", rendererList.ToArray());
            }
        }

        /// <summary>
        /// Loads an embedded resource from the calling assembly
        /// </summary>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] LoadFromResource(string resourcePath)
        {
            return GetResource(Assembly.GetCallingAssembly(), resourcePath);
        }

        /// <summary>
        /// Loads an embedded resource from an assembly
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] GetResource(Assembly assembly, string resourcePath)
        {
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        private static Texture2D defaultIcon = null;
        public static Texture2D GetDefaultIcon()
        {
            if (!defaultIcon)
            {
                try
                {
                    byte[] resource = LoadFromResource($"CustomNotes.Resources.Icons.default.png");
                    defaultIcon = LoadTextureRaw(resource);
                }
                catch { }
            }

            return defaultIcon;
        }

        private static Texture2D defaultCustomIcon = null;
        public static Texture2D GetDefaultCustomIcon()
        {
            if (!defaultCustomIcon)
            {
                try
                {
                    byte[] resource = LoadFromResource($"CustomNotes.Resources.Icons.defaultCustom.png");
                    defaultCustomIcon = LoadTextureRaw(resource);
                }
                catch { }
            }

            return defaultCustomIcon;
        }

        private static Texture2D errorIcon = null;
        public static Texture2D GetErrorIcon()
        {
            if (!errorIcon)
            {
                try
                {
                    byte[] resource = LoadFromResource($"CustomNotes.Resources.Icons.error.png");
                    errorIcon = LoadTextureRaw(resource);
                }
                catch { }
            }

            return errorIcon;
        }

        /// <summary>
        /// Loads an Texture2D from byte[]
        /// </summary>
        /// <param name="file"></param>
        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Length > 0)
            {
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(file))
                {
                    return texture;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets every file matching the filter in a path.
        /// </summary>
        /// <param name="path">Directory to search in.</param>
        /// <param name="filters">Pattern(s) to search for.</param>
        /// <param name="searchOption">Search options.</param>
        /// <param name="returnShortPath">Remove path from filepaths.</param>
        public static IEnumerable<string> GetFileNames(string path, IEnumerable<string> filters, SearchOption searchOption, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            foreach (string filter in filters)
            {
                IEnumerable<string> directoryFiles = Directory.GetFiles(path, filter, searchOption);

                if (returnShortPath)
                {
                    foreach (string directoryFile in directoryFiles)
                    {
                        string filePath = directoryFile.Replace(path, "");
                        if (filePath.Length > 0 && filePath.StartsWith(@"\"))
                        {
                            filePath = filePath.Substring(1, filePath.Length - 1);
                        }

                        if (!string.IsNullOrWhiteSpace(filePath) && !filePaths.Contains(filePath))
                        {
                            filePaths.Add(filePath);
                        }
                    }
                }
                else
                {
                    filePaths = filePaths.Union(directoryFiles).ToList();
                }
            }

            return filePaths.Distinct();
        }

        /// <summary>
        /// Safely unescape \n and \t
        /// </summary>
        /// <param name="text"></param>
        public static string SafeUnescape(string text)
        {
            string unescapedString;

            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    unescapedString = string.Empty;
                }
                else
                {
                    // Unescape just some of the basic formatting characters
                    unescapedString = text;
                    unescapedString = unescapedString.Replace("\\n", "\n");
                    unescapedString = unescapedString.Replace("\\t", "\t");
                }
            }
            catch
            {
                unescapedString = text;
            }

            return unescapedString;
        }

        /// <summary>
        /// Check if an IDifficultyBeatmap requires noodle extensions
        /// </summary>
        /// <param name="level"></param>
        public static bool IsNoodleMap(IDifficultyBeatmap level)
        {
            // thanks kinsi
            if (PluginManager.EnabledPlugins.Any(x => x.Name == "SongCore") && PluginManager.EnabledPlugins.Any(x => x.Name == "NoodleExtensions"))
            {
                bool isIsNoodleMap = SongCore.Collections.RetrieveDifficultyData(level)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Noodle Extensions") == true;
                return isIsNoodleMap;
            }
            else return false;
        }

        /// <summary>
        /// Get the proper note size from the config
        /// </summary>
        /// <param name="level"></param>
        public static float NoteSizeFromConfig(PluginConfig config)
        {
            // Not April Fools Day Code
            System.DateTime time;
            bool bunbundai = false;
            if (IPA.Utilities.Utils.CanUseDateTimeNowSafely)
                time = System.DateTime.Now;
            else
                time = System.DateTime.UtcNow;
            if ((time.Month == 4 && time.Day == 1) || bunbundai) return UnityEngine.Random.Range(0.25f, 1.5f);
            else return config.NoteSize;
        }
    }
}
