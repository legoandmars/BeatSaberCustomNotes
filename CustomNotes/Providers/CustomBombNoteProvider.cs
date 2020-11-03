using System;
using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using SiraUtil.Interfaces;
using CustomNotes.Managers;
using CustomNotes.Utilities;

namespace CustomNotes.Providers
{
    internal class CustomBombNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomBombNoteDecorator);

        public int Priority { get; set; } = 300;

        private class CustomBombNoteDecorator : IPrefabProvider<BombNoteController>
        {
            public bool Chain => true;

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container)
            {
                if (_noteAssetLoader.SelectedNote != 0)
                {
                    var note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.bomb").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
                }
            }

            public BombNoteController Modify(BombNoteController original)
            {
                original.gameObject.AddComponent<CustomBombController>();
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