using System;
using UnityEngine;
using SiraUtil.Interfaces;
using CustomNotes.Managers;

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
                original.gameObject.AddComponent<CustomBombController>();
                return original;
            }
        }
    }
}