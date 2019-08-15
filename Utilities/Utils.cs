using UnityEngine;
using IPA.Loader;
using IPA.Old;

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
        /// Sets the active state of a primary GameObject and the sets the opposite active state for the secondary GameObject.
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
        /// Check if a BSIPA plugin is enabled
        /// </summary>
        /// <param name="PluginName">Name or Id to search for</param>
        public static bool IsPluginEnabled(string PluginName)
        {
            if (IsPluginPresent(PluginName))
            {
                PluginLoader.PluginInfo pluginInfo = PluginManager.GetPluginFromId(PluginName);
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
