﻿using IPA.Loader;
using IPA.Old;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
        /// <param name="returnShortPath">Remove path from filepaths.</param>
        public static IEnumerable<string> GetFileNames(string path, IEnumerable<string> filters, SearchOption searchOption, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            foreach (string filter in filters)
            {
                if (returnShortPath)
                {
                    foreach (string directoryFile in Directory.GetFiles(path, filter, searchOption))
                    {
                        string filePath = directoryFile.Replace(path, "");
                        if (filePath.StartsWith(@"\") && filePath.Length > 0)
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
                    filePaths = filePaths.Union(Directory.GetFiles(path, filter, searchOption)).ToList();
                }
            }

            return filePaths.Distinct();
        }

        /// <summary>
        /// Check if a BSIPA plugin is enabled.
        /// </summary>
        /// <param name="PluginName">Name or Id to search for.</param>
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
        /// Check if a plugin exists.
        /// </summary>
        /// <param name="PluginName">Name or Id to search for.</param>
        public static bool IsPluginPresent(string PluginName)
        {
            // Check in BSIPA
            if (PluginManager.GetPlugin(PluginName) != null
                || PluginManager.GetPluginFromId(PluginName) != null)
            {
                return true;
            }

#pragma warning disable CS0618 // IPA is obsolete
            // Check in old IPA
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                if (plugin.Name == PluginName)
                {
                    return true;
                }
            }
#pragma warning restore CS0618 // IPA is obsolete

            return false;
        }
    }
}
