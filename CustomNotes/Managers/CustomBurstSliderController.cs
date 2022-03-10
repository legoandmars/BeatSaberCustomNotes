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
    public class CustomBurstSliderController : MonoBehaviour, IColorable, INoteControllerNoteWasCutEvent, INoteControllerNoteWasMissedEvent, INoteControllerDidInitEvent, INoteControllerNoteDidDissolveEvent
    {
        private PluginConfig _pluginConfig;

        protected Transform noteCube;
        private CustomNote _customNote;
        private BurstSliderGameNoteController _burstSliderGameNoteController;
        private CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool activePool;

        private SiraPrefabContainer.Pool _leftBurstSliderPool;
        private SiraPrefabContainer.Pool _rightBurstSliderPool;

        public Color Color
        {
            get => _customNoteColorNoteVisuals != null ? _customNoteColorNoteVisuals.noteColor : Color.white;
            set => SetColor(value);
        }

        [Inject]
        internal void Init(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            [Inject(Id = Protocol.LeftBurstSliderPool)] SiraPrefabContainer.Pool leftBurstSliderPool,
            [Inject(Id = Protocol.RightBurstSliderPool)] SiraPrefabContainer.Pool rightBurstSliderPool)
        {
            _pluginConfig = pluginConfig;
            _leftBurstSliderPool = leftBurstSliderPool;
            _rightBurstSliderPool = rightBurstSliderPool;

            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];

            _burstSliderGameNoteController = GetComponent<BurstSliderGameNoteController>();
            _customNoteColorNoteVisuals = gameObject.GetComponent<CustomNoteColorNoteVisuals>();
            _customNoteColorNoteVisuals.enabled = true;

            _burstSliderGameNoteController.didInitEvent.Add(this);
            _burstSliderGameNoteController.noteWasCutEvent.Add(this);
            _burstSliderGameNoteController.noteWasMissedEvent.Add(this);
            _burstSliderGameNoteController.noteDidDissolveEvent.Add(this);
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _burstSliderGameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            if (_pluginConfig.HMDOnly == false && LayerUtils.HMDOverride == false)
            {
                // only disable if custom notes display on both hmd and display
                noteMesh.forceRenderingOff = true;
            }
            else
            {
                noteMesh.gameObject.layer = (int)LayerUtils.NoteLayer.ThirdPerson;
            }
        }

        public void HandleNoteControllerNoteWasMissed(NoteController nc)
        {
            if (container != null)
                container.transform.SetParent(null);
            switch (nc.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    if (container != null)
                    {
                        container.Prefab.SetActive(false);
                        activePool?.Despawn(container);
                        container = null;
                    }
                    break;
                default:
                    break;
            }
        }

        public void HandleNoteControllerDidInit(NoteControllerBase noteController)
        {
            var data = noteController.noteData;
            SpawnThenParent(data.colorType == ColorType.ColorA
                ? _leftBurstSliderPool
                : _rightBurstSliderPool);
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
            container.Prefab.SetActive(true);
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

        private void Visuals_DidInit(ColorNoteVisuals visuals, NoteControllerBase noteController)
        {
            SetActiveThenColor(activeNote, (visuals as CustomNoteColorNoteVisuals).noteColor);
            // Hide certain parts of the default note which is not required
            if (_pluginConfig.HMDOnly == false && LayerUtils.HMDOverride == false)
            {
                _customNoteColorNoteVisuals.SetBaseGameVisualsLayer((int)LayerUtils.NoteLayer.Note);
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
                _customNoteColorNoteVisuals.SetBaseGameVisualsLayer((int)LayerUtils.NoteLayer.ThirdPerson);
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
            if (_burstSliderGameNoteController != null)
            {
                _burstSliderGameNoteController.didInitEvent.Remove(this);
                _burstSliderGameNoteController.noteWasCutEvent.Remove(this);
                _burstSliderGameNoteController.noteWasMissedEvent.Remove(this);
                _burstSliderGameNoteController.noteDidDissolveEvent.Remove(this);
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

        public void HandleNoteControllerNoteWasCut(NoteController nc, in NoteCutInfo _)
        {
            HandleNoteControllerNoteWasMissed(nc);
        }

        public void HandleNoteControllerNoteDidDissolve(NoteController noteController)
        {
            HandleNoteControllerNoteWasMissed(noteController);
        }
    }
}