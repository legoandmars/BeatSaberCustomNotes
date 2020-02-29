using CustomNotes.Settings.Utilities;
using IPA.Config;
using IPA.Config.Stores;

namespace CustomNotes.Settings
{
    public class Configuration
    {
        public static string CurrentlySelectedNote { get; internal set; }

        internal static void Init(Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
        }

        internal static void Load()
        {
            CurrentlySelectedNote = PluginConfig.Instance.lastNote;
        }

        internal static void Save()
        {
            PluginConfig.Instance.lastNote = CurrentlySelectedNote;
        }
    }
}
