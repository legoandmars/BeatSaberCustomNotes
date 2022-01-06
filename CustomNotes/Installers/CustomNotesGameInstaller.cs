using CustomNotes.Data;
using CustomNotes.Managers;
using CustomNotes.Overrides;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Extras;
using SiraUtil.Objects;
using SiraUtil.Objects.Beatmap;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace CustomNotes.Installers
{
    internal class CustomNotesGameInstaller : Installer
    {
        private readonly PluginConfig _pluginConfig;
        private readonly NoteAssetLoader _noteAssetLoader;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        private bool _shouldSetup;

        private const int DECORATION_PRIORITY = 300;

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
            if (_pluginConfig.Enabled && _noteAssetLoader.SelectedNote != 0 && _shouldSetup)
            {
                Container.BindInterfacesAndSelfTo<GameCameraManager>().AsSingle();
                Container.Bind<IInitializable>().To<CustomNoteManager>().AsSingle();
                CustomNote note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];

                #region Note Setup

                Container.RegisterRedecorator(new BasicNoteRegistration(DecorateNote, DECORATION_PRIORITY));
                MaterialSwapper.GetMaterials();
                MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteRight);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotRight);
                Utils.AddMaterialPropertyBlockController(note.NoteLeft);
                Utils.AddMaterialPropertyBlockController(note.NoteRight);
                Utils.AddMaterialPropertyBlockController(note.NoteDotLeft);
                Utils.AddMaterialPropertyBlockController(note.NoteDotRight);

                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.LeftArrowPool).WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteLeft));
                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.RightArrowPool).WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteRight));
                if (note.NoteDotLeft != null)
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.LeftDotPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
                if (note.NoteDotRight != null)
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.RightDotPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));

                #endregion

                #region Bomb Setup

                Container.RegisterRedecorator(new BombNoteRegistration(DecorateBombs, DECORATION_PRIORITY));

                if (note.NoteBomb != null)
                {
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.BombPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
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