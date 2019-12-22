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
                    ScoreSubmission.ProlongedDisableSubmission(Plugin.PluginName);
                    ScoreIsBlocked = true;
                    Logger.log.Info("ScoreSubmission has been disabled.");
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
                    ScoreSubmission.RemoveProlongedDisable(Plugin.PluginName);
                    ScoreIsBlocked = false;
                    Logger.log.Info("ScoreSubmission has been re-enabled.");
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
                    ScoreSubmission.RemoveProlongedDisable(Plugin.PluginName);
                    ScoreIsBlocked = false;
                    Logger.log.Info("Plugin is exiting, ScoreSubmission has been re-enabled.");
                }

                if (ScoreBlockList.Count != 0)
                {
                    ScoreBlockList.Clear();
                }
            }
        }
    }
}
