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
    public class CustomNoteController : MonoBehaviour, IColorable
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

            _gameNoteController.noteWasCutEvent += WasCut;
            _gameNoteController.noteWasMissedEvent += DidFinish;
            _gameNoteController.didInitEvent += Controller_DidInit;
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _gameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            if (_pluginConfig.HMDOnly == false && LayerUtils.HMDOverride == false)
            {
                // only disable if custom notes display on both hmd and display
                noteMesh.forceRenderingOff = true;
            }
        }

        private void DidFinish(NoteController nc)
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

        private void WasCut(NoteController nc, NoteCutInfo _)
        {
            DidFinish(nc);
        }

        private void Controller_DidInit(NoteController noteController)
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
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * _pluginConfig.NoteSize;
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
                if (LayerUtils.CameraSet == false)
                {
                    LayerUtils.SetCamera(Camera.main, LayerUtils.CameraView.FirstPerson);
                    LayerUtils.SetWatermark();
                }
            }
            else
            {
                LayerUtils.SetLayer(activeNote, LayerUtils.NoteLayer.Note);
                if (LayerUtils.CameraSet == false) LayerUtils.SetCamera(Camera.main, LayerUtils.CameraView.Default);
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
                if (_customNote.Descriptor.DisableBaseNoteArrows)
                {
                    _customNoteColorNoteVisuals.TurnOffVisuals();
                }
                else if (_pluginConfig.NoteSize != 1)
                {
                    _customNoteColorNoteVisuals.ScaleVisuals(_pluginConfig.NoteSize);
                }
            }
            else
            {
                if (!_customNote.Descriptor.DisableBaseNoteArrows)
                {
                    if (_pluginConfig.NoteSize != 1)
                    {
                        // arrows should be enabled in both views, with fake arrows rescaled
                        _customNoteColorNoteVisuals.CreateAndScaleFakeVisuals((int)LayerUtils.NoteLayer.FirstPerson, _pluginConfig.NoteSize);
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