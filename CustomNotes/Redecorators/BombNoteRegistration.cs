using SiraUtil.Objects;
using System;

namespace CustomNotes.Redecorators
{
    // I'm going to add this to SiraUtil soon, so you can swap to that when it's ready, but I don't want to delay that release. -SiraUtil
    internal sealed class BombNoteRegistration : TemplateRedecoratorRegistration<BombNoteController, BeatmapObjectsInstaller>
    {
        public BombNoteRegistration(Func<BombNoteController, BombNoteController> redecorateCall, int priority = 0, bool chain = true) : base("_bombNotePrefab", redecorateCall, priority, chain) { }
    }
}