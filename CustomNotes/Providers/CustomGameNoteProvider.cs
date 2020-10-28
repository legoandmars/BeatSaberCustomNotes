using System;
using UnityEngine;
using SiraUtil.Interfaces;

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
                var obj = new GameObject("Custom Game Note");
                obj.transform.SetParent(original.transform);
                return original;
            }
        }
    }
}