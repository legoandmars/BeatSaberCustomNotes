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
    }
}
