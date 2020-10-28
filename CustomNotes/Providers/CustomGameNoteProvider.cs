using System;
using SiraUtil.Interfaces;
using CustomNotes.Managers;

namespace CustomNotes.Providers
{
    internal class CustomGameNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomGameNoteDecorator);
        public int Priority { get; set; } = 300;

        private class CustomGameNoteDecorator : IPrefabProvider<GameNoteController>
        {
            public bool Chain => true;

            public GameNoteController Modify(GameNoteController original)
            {
                original.gameObject.AddComponent<CustomNoteController>();
                return original;
            }
        }
    }
}