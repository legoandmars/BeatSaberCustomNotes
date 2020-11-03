using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using CustomNotes.Data;
using CustomNotes.Overrides;
using CustomNotes.Utilities;

namespace CustomNotes.Managers
{
    public class CustomNoteController : MonoBehaviour
    {
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

        [Inject]
        internal void Init(NoteAssetLoader noteAssetLoader,
            [Inject(Id = "cn.left.arrow")] SiraPrefabContainer.Pool leftArrowNotePool,
            [Inject(Id = "cn.right.arrow")] SiraPrefabContainer.Pool rightArrowNotePool,
            [InjectOptional(Id = "cn.left.dot")] SiraPrefabContainer.Pool leftDotNotePool,
            [InjectOptional(Id = "cn.right.dot")] SiraPrefabContainer.Pool rightDotNotePool)
        {            
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

        private void DidFinish(NoteController nc)
        {
            container.transform.SetParent(null);
            switch (nc.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    activePool?.Despawn(container);
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
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }

        private void SpawnThenParent(SiraPrefabContainer.Pool noteModelPool)
        {
            container = noteModelPool.Spawn();
            activeNote = container.Prefab;
            activePool = noteModelPool;
            ParentNote(activeNote);
        }

        protected void SetActiveThenColor(GameObject note, Color color)
        {
            note.SetActive(true);
            Utils.ColorizeCustomNote(color, _customNote.Descriptor.NoteColorStrength, note);
        }

        private void Visuals_DidInit(ColorNoteVisuals visuals, NoteController noteController)
        {
            SetActiveThenColor(activeNote, visuals.noteColor);

            // Hide certain parts of the default note which is not required
            if (_customNote.Descriptor.DisableBaseNoteArrows)
            {
                _customNoteColorNoteVisuals.TurnOffVisuals();
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
    }
}