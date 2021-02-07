﻿using CustomNotes.Data;
using CustomNotes.Overrides;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Interfaces;
using SiraUtil.Objects;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    public class CustomNoteController : CustomNoteControllerBase
    {
        [Inject]
        internal void Init(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            [Inject(Id = "cn.left.arrow")] SiraPrefabContainer.Pool leftArrowNotePool,
            [Inject(Id = "cn.right.arrow")] SiraPrefabContainer.Pool rightArrowNotePool,
            [InjectOptional(Id = "cn.left.dot")] SiraPrefabContainer.Pool leftDotNotePool,
            [InjectOptional(Id = "cn.right.dot")] SiraPrefabContainer.Pool rightDotNotePool)
        {
            _pluginConfig = pluginConfig;
            _noteSize = pluginConfig.NoteSize;
            _leftArrowNotePool = leftArrowNotePool;
            _rightArrowNotePool = rightArrowNotePool;

            _leftDotNotePool = leftDotNotePool ?? _leftArrowNotePool;
            _rightDotNotePool = rightDotNotePool ?? _rightArrowNotePool;

            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];

            _gameNoteController = GetComponent<GameNoteController>();
            _customNoteColorNoteVisuals = gameObject.AddComponent<CustomNoteColorNoteVisuals>();

            _gameNoteController.noteWasCutEvent += WasCut;
            _gameNoteController.noteWasMissedEvent += DidFinish;
            _gameNoteController.didInitEvent += Controller_DidInit;
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _gameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            noteMesh.forceRenderingOff = true;
        }
    }

    public abstract class CustomNoteControllerBase : MonoBehaviour, IColorable
    {
        protected PluginConfig _pluginConfig;

        protected Transform noteCube;
        protected CustomNote _customNote;
        protected NoteController _gameNoteController;
        protected CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;
        protected float _noteSize = 1f;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool activePool;

        protected SiraPrefabContainer.Pool _leftDotNotePool;
        protected SiraPrefabContainer.Pool _rightDotNotePool;
        protected SiraPrefabContainer.Pool _leftArrowNotePool;
        protected SiraPrefabContainer.Pool _rightArrowNotePool;

        public Color Color => _customNoteColorNoteVisuals != null ? _customNoteColorNoteVisuals.noteColor : Color.white;

        protected virtual void DidFinish(NoteController nc)
        {
            container.transform.SetParent(null);
            switch (nc.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    if(container != null)
                    {
                        activePool?.Despawn(container);
                        container = null;
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void WasCut(NoteController nc, NoteCutInfo _)
        {
            DidFinish(nc);
        }

        protected virtual void Controller_DidInit(NoteController noteController)
        {
            var data = noteController.noteData;
            SpawnThenParent(data.colorType == ColorType.ColorA
                ? data.cutDirection == NoteCutDirection.Any ? _leftDotNotePool : _leftArrowNotePool
                : data.cutDirection == NoteCutDirection.Any ? _rightDotNotePool : _rightArrowNotePool);
        }

        protected virtual void ParentNote(GameObject fakeMesh)
        {
            container.transform.SetParent(noteCube);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localRotation = Quaternion.identity;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * _noteSize;
            container.transform.localScale = Vector3.one;
        }

        protected virtual void SpawnThenParent(SiraPrefabContainer.Pool noteModelPool)
        {
            container = noteModelPool.Spawn();
            activeNote = container.Prefab;
            activePool = noteModelPool;
            ParentNote(activeNote);
        }

        protected virtual void SetActiveThenColor(GameObject note, Color color)
        {
            note.SetActive(true);
            if (_customNote.Descriptor.UsesNoteColor)
            {
                SetColor(color, true);
            }
        }

        protected virtual void Visuals_DidInit(ColorNoteVisuals visuals, NoteController noteController)
        {
            SetActiveThenColor(activeNote, visuals.noteColor);
            // Hide certain parts of the default note which is not required
            if (_customNote.Descriptor.DisableBaseNoteArrows)
            {
                _customNoteColorNoteVisuals.TurnOffVisuals();
            }
            else if(_noteSize != 1)
            {
                _customNoteColorNoteVisuals.ScaleVisuals(_noteSize);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_gameNoteController != null)
            {
                _gameNoteController.noteWasCutEvent -= WasCut;
                _gameNoteController.noteWasMissedEvent -= DidFinish;
                _gameNoteController.didInitEvent -= Controller_DidInit;
            }
            if (_customNoteColorNoteVisuals != null)
            {
                _customNoteColorNoteVisuals.didInitEvent -= Visuals_DidInit;
            }
        }

        public void SetColor(Color color)
        {
            SetColor(color, false);
        }

        public void SetColor(Color color, bool updateMatBlocks)
        {
            if (activeNote != null)
            {
                _customNoteColorNoteVisuals.SetColor(color, updateMatBlocks);
                Utils.ColorizeCustomNote(color, _customNote.Descriptor.NoteColorStrength, activeNote);
            }
        }
    }
}