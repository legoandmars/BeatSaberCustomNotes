using System.Collections.Generic;
using BS_Utils.Gameplay;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes.Utilities
{
    internal class ScoreUtility
    {
        private static List<string> ScoreBlockList = new List<string>();
        private static bool ScoreIsBlocked = false;
        private static object acquireLock = new object();

        internal static void DisableScoreSubmission(string BlockedBy)
        {
            lock (acquireLock)
            {
                if (!ScoreBlockList.Contains(BlockedBy))
                {
                    ScoreBlockList.Add(BlockedBy);
                }

                if (!ScoreIsBlocked)
                {
                    Logger.Log("ScoreSubmission has been disabled.", LogLevel.Notice);
                    ScoreSubmission.ProlongedDisableSubmission(Plugin.PluginName);
                    ScoreIsBlocked = true;
                }
            }
        }

        internal static void EnableScoreSubmission(string BlockedBy)
        {
            lock (acquireLock)
            {
                if (ScoreBlockList.Contains(BlockedBy))
                {
                    ScoreBlockList.Remove(BlockedBy);
                }

                if (ScoreIsBlocked && ScoreBlockList.Count == 0)
                {
                    Logger.Log("ScoreSubmission has been re-enabled.", LogLevel.Notice);
                    ScoreSubmission.RemoveProlongedDisable(Plugin.PluginName);
                    ScoreIsBlocked = false;
                }
            }
        }

        /// <summary>
        /// Should only be called on plugin exit!
        /// </summary>
        internal static void Cleanup()
        {
            lock (acquireLock)
            {
                if (ScoreIsBlocked)
                {
                    Logger.Log("Plugin is exiting, ScoreSubmission has been re-enabled.", LogLevel.Notice);
                    ScoreSubmission.RemoveProlongedDisable(Plugin.PluginName);
                    ScoreIsBlocked = false;
                }

                if (ScoreBlockList.Count != 0)
                {
                    ScoreBlockList.Clear();
                }
            }
        }
    }
}
