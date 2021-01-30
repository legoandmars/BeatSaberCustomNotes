namespace CustomNotes.Settings.Utilities
{
    public class PluginConfig
    {
        public virtual string LastNote { get; set; }

        public virtual float NoteSize { get; set; } = 1;

        public virtual bool OtherPlayerMultiplayerNotes { get; set; } = true;

        public virtual bool RandomMultiplayerNotes { get; set; } = true;

        public virtual bool RandomnessIsConsistentPerPlayer { get; set; } = true;

        public virtual bool SyncNotesInMultiplayer { get; set; } = true;
    }
}