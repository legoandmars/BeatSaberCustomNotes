using BS_Utils.Gameplay;
using System.Collections.Generic;

namespace CustomNotes.Utilities
{
    public class ScoreUtility
    {
        private static readonly object acquireLock = new object();
        private static readonly IList<string> scoreBlockList = new List<string>();

        public static bool ScoreIsBlocked { get; private set; } = false;

        internal static void DisableScoreSubmission(string blockedBy)
        {
            lock (acquireLock)
            {
                if (!scoreBlockList.Contains(blockedBy))
                {
                    scoreBlockList.Add(blockedBy);
                }

                if (!ScoreIsBlocked)
                {
                    ScoreSubmission.ProlongedDisableSubmission(Plugin.PluginName);
                    ScoreIsBlocked = true;
                    Logger.log.Info("ScoreSubmission has been disabled.");
                }
            }
        }

        internal static void EnableScoreSubmission(string blockedBy)
        {
            lock (acquireLock)
            {
                if (scoreBlockList.Contains(blockedBy))
                {
                    scoreBlockList.Remove(blockedBy);
                }

                if (ScoreIsBlocked && scoreBlockList.Count == 0)
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

                if (scoreBlockList.Count != 0)
                {
                    scoreBlockList.Clear();
                }
            }
        }
    }
}
