using CustomNotes.Settings.Utilities;
using IPA.Config;
using IPA.Utilities;

namespace CustomNotes.Settings
{
    public class Configuration
    {
        private static Ref<PluginConfig> config;
        private static IConfigProvider configProvider;

        public static string CurrentlySelectedNote { get; internal set; }

        internal static void Init(IConfigProvider cfgProvider)
        {
            configProvider = cfgProvider;
            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                }
                config = v;
            });
        }

        internal static void Load()
        {
            CurrentlySelectedNote = config.Value.lastNote;
        }

        internal static void Save()
        {
            config.Value.lastNote = CurrentlySelectedNote;

            configProvider.Store(config.Value);
        }
    }
}
