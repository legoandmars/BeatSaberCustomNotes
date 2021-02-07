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
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container, GameplayCoreSceneSetupData sceneSetupData)
            {
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows) || !Container.HasBinding<MultiplayerLevelSceneSetupData>();
                if (_noteAssetLoader.SelectedNote != 0 && CanSetup)
                {
                    var note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    if(note.NoteBomb != null)
                    {
                        MaterialSwapper.GetMaterials();
                        MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
                        Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.bomb").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
                    }
                }
            }

            public BombNoteController Modify(BombNoteController original)
            {
                if (!CanSetup) return original;
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