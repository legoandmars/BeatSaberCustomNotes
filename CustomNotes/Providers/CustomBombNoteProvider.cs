using System;
using UnityEngine;
using SiraUtil.Interfaces;

namespace CustomNotes.Providers
{
    internal class CustomBombNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomBombNoteDecorator);

        public int Priority { get; set; } = 300;

        private class CustomBombNoteDecorator : IPrefabProvider<BombNoteController>
        {
            public bool Chain => true;

            public BombNoteController Modify(BombNoteController original)
            {
                var obj = new GameObject("Custom Bomb Note");
                obj.transform.SetParent(original.transform);
                return original;
            }
        }
    }
}