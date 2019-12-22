using IPA.Loader;
using IPA.Old;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CustomNotes.Utilities
{
    public class Utils
    {
        /// <summary>
        /// Sets the active state of a GameObject.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="setActive">Desired state</param>
        public static bool ActivateGameObject(GameObject gameObject, bool setActive) => ActivateGameObject(gameObject, setActive, null);

        /// <summary>
        /// Sets the active state of a primary GameObject and then sets the opposite active state for the secondary GameObject.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="setActive">Desired state</param>
        /// <param name="secondary"></param>
        public static bool ActivateGameObject(GameObject primary, bool setActive, GameObject secondary)
        {
            if (primary)
            {
                if (primary.activeSelf != setActive)
                {
                    primary.SetActive(setActive);
                }

                if (secondary != null)
                {
                    return ActivateGameObject(secondary, !setActive);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Colorize a Note based on a ColorManager and CustomNote configuration
        /// </summary>
        /// <param name="colorManager">ColorManager</param>
        /// <param name="noteType">Type of note</param>
        /// <param name="colorStrength">Color strength</param>
        /// <param name="noteObject">Note to colorize</param>
        public static void ColorizeCustomNote(ColorManager colorManager, NoteType noteType, float colorStrength, GameObject noteObject)
        {
            if (!noteObject || !colorManager)
            {
                return;
            }

            Color noteColor = colorManager.ColorForNoteType(noteType) * colorStrength;
            int numberOfChildren = noteObject.GetComponentsInChildren<Transform>().Length;

            for (int i = 0; i < numberOfChildren; i++)
            {
                DisableNoteColorOnGameobject colorDisabled = noteObject.GetComponentsInChildren<Transform>()[i].GetComponent<DisableNoteColorOnGameobject>();
                if (!colorDisabled)
                {
                    Renderer childRenderer = noteObject.GetComponentsInChildren<Transform>()[i].GetComponent<Renderer>();
                    if (childRenderer)
                    {
                        childRenderer.material.SetColor("_Color", noteColor);
                    }
                }
            }
        }

        /// <summary>
        /// Validate path.
        /// </summary>
        /// <param name="path">Path to validate.</param>
        /// <param name="allowRelativePaths">Allow relative paths.</param>
        public static bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid;

            try
            {
                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = !string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' }));
                }
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Gets every file matching the filter in a path.
        /// </summary>
        /// <param name="path">Directory to search in.</param>
        /// <param name="filters">Pattern(s) to search for.</param>
        /// <param name="searchOption">Search options.</param>
        /// <param name="fullPath">Keep filepaths.</param>
        public static IEnumerable<string> GetFileNames(string path, IEnumerable<string> filters, SearchOption searchOption, bool fullPath = false)
        {
            List<string> filePaths = new List<string>();
            foreach (string filter in filters)
            {
                filePaths.AddRange(Directory.GetFiles(path, filter, searchOption));
            }

            if (fullPath)
            {
                return filePaths.Distinct();
            }

            IList<string> fileNames = new List<string>();
            foreach (string filePath in filePaths)
            {
                fileNames.Add(Path.GetFileName(filePath));
            }

            return fileNames.Distinct();
        }

        /// <summary>
        /// Check if a BSIPA plugin is enabled
        /// </summary>
        /// <param name="PluginName">Name or Id to search for</param>
        public static bool IsPluginEnabled(string PluginName)
        {
            if (IsPluginPresent(PluginName))
            {
                PluginLoader.PluginInfo pluginInfo = PluginManager.GetPlugin(PluginName);
                if (pluginInfo?.Metadata == null)
                {
                    pluginInfo = PluginManager.GetPluginFromId(PluginName);
                }

                if (pluginInfo?.Metadata != null)
                {
                    return PluginManager.IsEnabled(pluginInfo.Metadata);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a plugin exists
        /// </summary>
        /// <param name="PluginName">Name or Id to search for</param>
        public static bool IsPluginPresent(string PluginName)
        {
            // Check in BSIPA
            if (PluginManager.GetPlugin(PluginName) != null ||
                PluginManager.GetPluginFromId(PluginName) != null)
            {
                return true;
            }

#pragma warning disable CS0618 // IPA is obsolete
            // Check in old IPA
            foreach (IPlugin p in PluginManager.Plugins)
            {
                if (p.Name == PluginName)
                {
                    return true;
                }
            }
#pragma warning restore CS0618 // IPA is obsolete

            return false;
        }
    }
}
