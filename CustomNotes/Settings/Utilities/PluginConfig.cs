namespace CustomNotes.Settings.Utilities
{
    public class PluginConfig
    {
        public virtual string LastNote { get; set; }

        public virtual float NoteSize { get; set; } = 1;

        public virtual bool HMDOnly { get; set; } = false;

        public virtual bool DisableOnNoodle { get; set; } = true;

        public virtual bool DisableOnModifier { get; set; } = true;
    }
}