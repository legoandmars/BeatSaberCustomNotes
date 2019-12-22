using IPA.Loader;
using IPA.Old;
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
