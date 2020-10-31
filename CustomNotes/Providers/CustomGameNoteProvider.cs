using System;
using Zenject;
using UnityEngine;
using SiraUtil.Objects;
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

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container)
            {
                if (_noteAssetLoader.SelectedNote != 0)
                {
                    var note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    var go = new GameObject("CustomNotesInitialPrefabs");

                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.left.arrow").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteLeft));
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.left.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.right.arrow").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteRight));
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.right.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));
                }
            }

            public GameNoteController Modify(GameNoteController original)
            {
                original.gameObject.AddComponent<CustomNoteController>();
                return original;
            }

            private SiraPrefabContainer NotePrefabContainer(GameObject initialPrefab)
            {
                var prefab = new GameObject("CustomNotes" + initialPrefab.name).AddComponent<SiraPrefabContainer>();
                prefab.Prefab = initialPrefab;
                return prefab;
            }
        }
    }
}