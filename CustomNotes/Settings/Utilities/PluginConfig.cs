namespace CustomNotes.Settings.Utilities
{
    public class PluginConfig
    {
        public virtual string LastNote { get; set; }

        public virtual float NoteSize { get; set; } = 1;

        public virtual bool NoteMirrorReflection { get; set; } = true;
    }
}