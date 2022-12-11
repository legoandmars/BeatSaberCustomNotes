﻿using CustomNotes.Data;
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
                Container.BindInterfacesAndSelfTo<WatermarkManager>().AsSingle();
                Container.Bind<IInitializable>().To<CustomNoteManager>().AsSingle();
                CustomNote note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];

                #region Note Setup

                if (_gameplayCoreSceneSetupData.gameplayModifiers.proMode)
                {
                    // Use the pro mode decorator
                    Container.RegisterRedecorator(new ProModeNoteRegistration(DecorateNote, DECORATION_PRIORITY));
                }
                else
                {
                    Container.RegisterRedecorator(new BasicNoteRegistration(DecorateNote, DECORATION_PRIORITY));
                }
                
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

                #region Burst Slider Setup

                Container.RegisterRedecorator(new BurstSliderNoteRegistration(DecorateBurstSliderFill, DECORATION_PRIORITY));

                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderRight);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderLeft);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderRight);

                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.LeftBurstSliderPool).WithInitialSize(40).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderLeft));
                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.RightBurstSliderPool).WithInitialSize(40).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderRight));

                #endregion

                #region Burst Slider Head Setup

                Container.RegisterRedecorator(new BurstSliderHeadNoteRegistration(DecorateNote, DECORATION_PRIORITY));

                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderHeadLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderHeadRight);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderHeadDotLeft);
                MaterialSwapper.ReplaceMaterialsForGameObject(note.BurstSliderHeadDotRight);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderHeadLeft);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderHeadRight);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderHeadDotLeft);
                Utils.AddMaterialPropertyBlockController(note.BurstSliderHeadDotRight);

                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.LeftBurstSliderHeadPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderHeadLeft));
                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.RightBurstSliderHeadPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderHeadRight));
                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.LeftBurstSliderHeadDotPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderHeadDotLeft));
                Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId(Protocol.RightBurstSliderHeadDotPool).WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.BurstSliderHeadDotRight));

                #endregion

                #region Slider Layer

                if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
                {
                    Container.RegisterRedecorator(new ShortSliderNoteRegistration(DecorateSlider, DECORATION_PRIORITY));
                    Container.RegisterRedecorator(new MediumSliderNoteRegistration(DecorateSlider, DECORATION_PRIORITY));
                    Container.RegisterRedecorator(new LongSliderNoteRegistration(DecorateSlider, DECORATION_PRIORITY));
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

        public BurstSliderGameNoteController DecorateBurstSliderFill(BurstSliderGameNoteController original)
        {
            if (!_shouldSetup)
                return original;

            original.gameObject.AddComponent<CustomBurstSliderController>();

            ColorNoteVisuals originalVisuals = original.GetComponent<ColorNoteVisuals>();

            CustomNoteColorNoteVisuals customVisuals = original.gameObject.AddComponent<CustomNoteColorNoteVisuals>();
            customVisuals.enabled = false;
            foreach (FieldInfo info in originalVisuals.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                info.SetValue(customVisuals, info.GetValue(originalVisuals));
            UnityEngine.Object.Destroy(originalVisuals);

            return original;
        }

        public SliderController DecorateSlider(SliderController original)
        {
            original.transform.Find("MeshRenderer").gameObject.layer = 0;
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