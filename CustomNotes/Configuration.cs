using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes
{
    internal static class Configuration
    {
        internal static string CurrentlySelectedNote;

        internal static void Load()
        {
            CurrentlySelectedNote = Plugin.config.Value.lastNote;
        }

        internal static void Save()
        {
            Plugin.config.Value.lastNote = CurrentlySelectedNote;

            Plugin.configProvider.Store(Plugin.config.Value);
        }
    }
}
