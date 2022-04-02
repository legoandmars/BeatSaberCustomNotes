namespace CustomNotes.Settings.Utilities
{
    public class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual string LastNote { get; set; }

        public virtual float NoteSize { get; set; } = 1;

        public virtual bool HMDOnly { get; set; } = false;

        public virtual bool AutoDisable { get; set; } = false;

        public virtual bool DisableAprilFools { get; set; } = false;
    }
}