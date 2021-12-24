using Zenject;
using CustomNotes.Managers;
using SiraUtil.Extras;
using SiraUtil.Objects.Beatmap;
using System;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Objects;
using UnityEngine;
using CustomNotes.Data;
using System.Reflection;
using CustomNotes.Overrides;

namespace CustomNotes.Installers
{
    internal class CustomNotesGameInstaller : Installer
    {
        private readonly PluginConfig _pluginConfig;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        private bool _shouldSetup;

        public CustomNotesGameInstaller(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public override void InstallBindings()
        {
            bool autoDisable = _pluginConfig.AutoDisable && (_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes || _gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows || _gameplayCoreSceneSetupData.gameplayModifiers.smallCubes || Utils.IsNoodleMap(_gameplayCoreSceneSetupData.difficultyBeatmap));
            _shouldSetup = !autoDisable && (!(_gameplayCoreSceneSetupData.gameplayModifiers.ghostNotes || _gameplayCoreSceneSetupData.gameplayModifiers.disappearingArrows) || !Container.HasBinding<MultiplayerLevelSceneSetupData>());
            if (_noteAssetLoader.SelectedNote != 0)
            {
                Container.BindInterfacesAndSelfTo<GameCameraManager>().AsSingle();
                Container.Bind<IInitializable>().To<CustomNoteManager>().AsSingle();
                CustomNote note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];

                #region Note Setup

                Container.RegisterRedecorator(new BasicNoteRegistration(DecorateNote, 300));
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
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.left.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
                if (note.NoteDotRight != null)
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.right.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));

                #endregion

                #region Bomb Setup

                if (note.NoteBomb != null)
                {
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.bomb").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
                }

                #endregion
            }
        }

        private GameNoteController DecorateNote(GameNoteController original)
        {
            if (!_shouldSetup)
                return original;

            original.gameObject.AddComponent<CustomNoteController>();

            ColorNoteVisuals originalVisuals = original.GetComponent<ColorNoteVisuals>();

            CustomNoteColorNoteVisuals customVisuals = original.gameObject.AddComponent<CustomNoteColorNoteVisuals>();
            customVisuals.enabled = false;
            foreach (FieldInfo info in originalVisuals.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                info.SetValue(customVisuals, info.GetValue(originalVisuals));
            UnityEngine.Object.Destroy(originalVisuals);

            return original;
        }

        public BombNoteController DecorateBombs(BombNoteController original)
        {
            if (!_shouldSetup)
                return original;

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