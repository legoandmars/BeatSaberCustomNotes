using CustomNotes.Settings.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNotes.Utilities.MultiplayerCustomNoteModeExtensions
{
    public enum MultiplayerCustomNoteMode
    {
        None,
        SameAsLocalPlayer,
        Random,
        RandomConsistent
    }
    public static class MultiplayerCustomNoteModeExtensions
    {
        private const string _none = "None";
        private const string _local = "Same as you";
        private const string _random = "Random";
        private const string _randomConsistent = "Random consistent";

        public static MultiplayerCustomNoteMode GetMultiplayerCustomNoteMode(this string modeString) {
            switch(modeString) {
                case _none:
                default:
                    return MultiplayerCustomNoteMode.None;
                case _local:
                    return MultiplayerCustomNoteMode.SameAsLocalPlayer;
                case _random:
                    return MultiplayerCustomNoteMode.Random;
                case _randomConsistent:
                    return MultiplayerCustomNoteMode.RandomConsistent;
            }
        }

        public static string ToSettingsString(this MultiplayerCustomNoteMode mode) {
            switch(mode) {
                case MultiplayerCustomNoteMode.None:
                default:
                    return _none;
                case MultiplayerCustomNoteMode.SameAsLocalPlayer:
                    return _local;
                case MultiplayerCustomNoteMode.Random:
                    return _random;
                case MultiplayerCustomNoteMode.RandomConsistent:
                    return _randomConsistent;
            }
        }

        public static void SetMultiNoteSettingFromString(PluginConfig pluginConfig, string modeString) {
            MultiplayerCustomNoteMode mode = modeString.GetMultiplayerCustomNoteMode();

            SetMultiNoteSetting(pluginConfig, mode);
        }

        public static void SetMultiNoteSetting(PluginConfig pluginConfig, MultiplayerCustomNoteMode mode) {
            switch (mode) {
                case MultiplayerCustomNoteMode.None:
                default:
                    pluginConfig.OtherPlayerMultiplayerNotes = false;
                    pluginConfig.RandomMultiplayerNotes = false;
                    pluginConfig.RandomnessIsConsistentPerPlayer = false;
                    break;
                case MultiplayerCustomNoteMode.SameAsLocalPlayer:
                    pluginConfig.OtherPlayerMultiplayerNotes = true;
                    pluginConfig.RandomMultiplayerNotes = false;
                    pluginConfig.RandomnessIsConsistentPerPlayer = false;
                    break;
                case MultiplayerCustomNoteMode.Random:
                    pluginConfig.OtherPlayerMultiplayerNotes = true;
                    pluginConfig.RandomMultiplayerNotes = true;
                    pluginConfig.RandomnessIsConsistentPerPlayer = false;
                    break;
                case MultiplayerCustomNoteMode.RandomConsistent:
                    pluginConfig.OtherPlayerMultiplayerNotes = true;
                    pluginConfig.RandomMultiplayerNotes = true;
                    pluginConfig.RandomnessIsConsistentPerPlayer = true;
                    break;
            }
        }

        public static string GetMultiNoteSettingString(PluginConfig pluginConfig) {
            if (!pluginConfig.OtherPlayerMultiplayerNotes)
                return MultiplayerCustomNoteMode.None.ToSettingsString();

            if (!pluginConfig.RandomMultiplayerNotes)
                return MultiplayerCustomNoteMode.SameAsLocalPlayer.ToSettingsString();

            if (!pluginConfig.RandomnessIsConsistentPerPlayer)
                return MultiplayerCustomNoteMode.Random.ToSettingsString();

            return MultiplayerCustomNoteMode.RandomConsistent.ToSettingsString();
        }
    }
}
