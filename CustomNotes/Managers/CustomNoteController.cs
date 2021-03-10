using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using CustomNotes.Data;
using SiraUtil.Interfaces;
using CustomNotes.Overrides;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;

namespace CustomNotes.Managers
{
    public class CustomNoteController : MonoBehaviour, IColorable, INoteControllerNoteWasCutEvent, INoteControllerNoteWasMissedEvent, INoteControllerDidInitEvent
    {
        private PluginConfig _pluginConfig;

        protected Transform noteCube;
        private CustomNote _customNote;
        private GameNoteController _gameNoteController;
        private CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool activePool;

        private SiraPrefabContainer.Pool _leftDotNotePool;
        private SiraPrefabContainer.Pool _rightDotNotePool;
        private SiraPrefabContainer.Pool _leftArrowNotePool;
        private SiraPrefabContainer.Pool _rightArrowNotePool;

        public Color Color => _customNoteColorNoteVisuals != null ? _customNoteColorNoteVisuals.noteColor : Color.white;

        [Inject]
        internal void Init(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            [Inject(Id = "cn.left.arrow")] SiraPrefabContainer.Pool leftArrowNotePool,
            [Inject(Id = "cn.right.arrow")] SiraPrefabContainer.Pool rightArrowNotePool,
            [InjectOptional(Id = "cn.left.dot")] SiraPrefabContainer.Pool leftDotNotePool,
            [InjectOptional(Id = "cn.right.dot")] SiraPrefabContainer.Pool rightDotNotePool)
        {
            _pluginConfig = pluginConfig;
            _leftArrowNotePool = leftArrowNotePool;
            _rightArrowNotePool = rightArrowNotePool;

            _leftDotNotePool = leftDotNotePool ?? _leftArrowNotePool;
            _rightDotNotePool = rightDotNotePool ?? _rightArrowNotePool;

            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];

            _gameNoteController = GetComponent<GameNoteController>();
            _customNoteColorNoteVisuals = gameObject.AddComponent<CustomNoteColorNoteVisuals>();

            _gameNoteController.didInitEvent.Add(this);
            _gameNoteController.noteWasMissedEvent.Add(this);
            _gameNoteController.noteWasCutEvent.Add(this);
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _gameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            if (_pluginConfig.HMDOnly == false && LayerUtils.HMDOverride == false)
            {
                // only disable if custom notes display on both hmd and display
                noteMesh.forceRenderingOff = true;
            }
            else
            {
                noteMesh.gameObject.layer = (int) LayerUtils.NoteLayer.ThirdPerson;
            }
        }

        public void HandleNoteControllerNoteWasMissed(NoteController nc)
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

        public void HandleNoteControllerNoteWasCut(NoteController nc, in NoteCutInfo _)
        {
            HandleNoteControllerNoteWasMissed(nc);
        }

        public void HandleNoteControllerDidInit(NoteController noteController)
        {
            var data = noteController.noteData;
            SpawnThenParent(data.colorType == ColorType.ColorA
                ? data.cutDirection == NoteCutDirection.Any ? _leftDotNotePool : _leftArrowNotePool
                : data.cutDirection == NoteCutDirection.Any ? _rightDotNotePool : _rightArrowNotePool);
        }

        private void ParentNote(GameObject fakeMesh)
        {
            container.transform.SetParent(noteCube);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localRotation = Quaternion.identity;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * Utils.NoteSizeFromConfig(_pluginConfig);
            container.transform.localScale = Vector3.one;
        }

        private void SpawnThenParent(SiraPrefabContainer.Pool noteModelPool)
        {
            container = noteModelPool.Spawn();
            activeNote = container.Prefab;
            activePool = noteModelPool;
            if (_pluginConfig.HMDOnly == true || LayerUtils.HMDOverride == true)
            {
                LayerUtils.SetLayer(activeNote, LayerUtils.NoteLayer.FirstPerson);
            }
            else
            {
                LayerUtils.SetLayer(activeNote, LayerUtils.NoteLayer.Note);
            }
            ParentNote(activeNote);
        }

        protected void SetActiveThenColor(GameObject note, Color color)
        {
            note.SetActive(true);
            if (_customNote.Descriptor.UsesNoteColor)
            {
                SetColor(color, true);
            }
        }

        private void Visuals_DidInit(ColorNoteVisuals visuals, NoteController noteController)
        {
            SetActiveThenColor(activeNote, visuals.noteColor);
            // Hide certain parts of the default note which is not required
            if(_pluginConfig.HMDOnly == false && LayerUtils.HMDOverride == false)
            {
                _customNoteColorNoteVisuals.SetBaseGameVisualsLayer((int) LayerUtils.NoteLayer.Note);
                if (_customNote.Descriptor.DisableBaseNoteArrows)
                {
                    _customNoteColorNoteVisuals.TurnOffVisuals();
                }
                else if (Utils.NoteSizeFromConfig(_pluginConfig) != 1)
                {
                    _customNoteColorNoteVisuals.ScaleVisuals(Utils.NoteSizeFromConfig(_pluginConfig));
                }
            }
            else
            {
                // HMDOnly code
                _customNoteColorNoteVisuals.SetBaseGameVisualsLayer((int) LayerUtils.NoteLayer.ThirdPerson);
                if (!_customNote.Descriptor.DisableBaseNoteArrows)
                {
                    if (Utils.NoteSizeFromConfig(_pluginConfig) != 1)
                    {
                        // arrows should be enabled in both views, with fake arrows rescaled
                        _customNoteColorNoteVisuals.CreateAndScaleFakeVisuals((int)LayerUtils.NoteLayer.FirstPerson, Utils.NoteSizeFromConfig(_pluginConfig));
                    }
                    else
                    {
                        // arrows should be enabled in both views
                        _customNoteColorNoteVisuals.CreateFakeVisuals((int)LayerUtils.NoteLayer.FirstPerson);
                    }
                }
            }
        }

        protected void OnDestroy()
        {
            if (_gameNoteController != null)
            {
                _gameNoteController.didInitEvent.Remove(this);
                _gameNoteController.noteWasMissedEvent.Remove(this);
                _gameNoteController.noteWasCutEvent.Remove(this);
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