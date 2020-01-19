using BS_Utils.Gameplay;
using System.Collections.Generic;

namespace CustomNotes.Utilities
{
    public class ScoreUtility
    {
        private static readonly IList<string> scoreBlockList = new List<string>();

        public static bool ScoreIsBlocked { get; private set; } = false;

        internal static void DisableScoreSubmission(string reason)
        {
            lock (scoreBlockList)
            {
                if (!scoreBlockList.Contains(reason))
                {
                    scoreBlockList.Add(reason);
                }

                if (!ScoreIsBlocked)
                {
                    ScoreSubmission.ProlongedDisableSubmission(Plugin.PluginName);
                    ScoreIsBlocked = true;
                    Logger.log.Info("ScoreSubmission has been disabled.");
                }
            }
        }

        internal static void EnableScoreSubmission(string reason)
        {
            lock (scoreBlockList)
            {
                if (scoreBlockList.Contains(reason))
                {
                    scoreBlockList.Remove(reason);
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
            lock (scoreBlockList)
            {
                if (ScoreIsBlocked)
                {
                    Logger.log.Info("Plugin is exiting, ScoreSubmission has been re-enabled.");
                    ScoreSubmission.RemoveProlongedDisable(Plugin.PluginName);
                    ScoreIsBlocked = false;
                }

                if (scoreBlockList.Count != 0)
                {
                    scoreBlockList.Clear();
                }
            }
        }
    }
}
