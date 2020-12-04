using System;
using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using SiraUtil.Interfaces;
using CustomNotes.Managers;
using CustomNotes.Utilities;

namespace CustomNotes.Providers
{
    internal class CustomGameNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomGameNoteDecorator);
        public int Priority { get; set; } = 300;

        private class CustomGameNoteDecorator : IPrefabProvider<GameNoteController>
        {
            public bool Chain => true;
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container, GameplayCoreSceneSetupData sceneSetupData)
            {
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows) || !Container.HasBinding<MultiplayerLevelSceneSetupData>();
                if (_noteAssetLoader.SelectedNote != 0)
                {
                    var note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteLeft);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteRight);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotLeft);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotRight);
                    Utils.AddMaterialPropertyBlockController(note.NoteLeft);
                    Utils.AddMaterialPropertyBlockController(note.NoteRight);
                    Utils.AddMaterialPropertyBlockController(note.NoteDotLeft);
                    Utils.AddMaterialPropertyBlockController(note.NoteDotRight);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.left.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteLeft));
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.right.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteRight));
                    if (note.NoteDotLeft != null)
                    {
                        Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.left.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
                    }
                    if (note.NoteDotRight != null)
                    {
                        Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.right.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));
                    }
                }
            }

            public GameNoteController Modify(GameNoteController original)
            {
                if (!CanSetup) return original;
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