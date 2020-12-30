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
    public class CustomMultiplayerConnectedPlayerGameNoteController : CustomNoteControllerBase
    {

        [Inject]
        private CustomMultiplayerNoteEventManager _customMultiplayerNoteEventManager;

        [Inject]
        internal void Init(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            DiContainer diContainer,
            [Inject(Id = "cn.multi.left.arrow")] SiraPrefabContainer.Pool leftArrowNotePool,
            [Inject(Id = "cn.multi.right.arrow")] SiraPrefabContainer.Pool rightArrowNotePool,
            [InjectOptional(Id = "cn.multi.left.dot")] SiraPrefabContainer.Pool leftDotNotePool,
            [InjectOptional(Id = "cn.multi.right.dot")] SiraPrefabContainer.Pool rightDotNotePool,
            [Inject(Id = "cn.multi.note")] CustomNote selectedNote,
            CustomMultiplayerNoteEventManager customMultiplayerNoteEventManager)
        {
            _leftArrowNotePool = leftArrowNotePool;
            _rightArrowNotePool = rightArrowNotePool;
            _leftDotNotePool = leftDotNotePool;
            _rightDotNotePool = rightDotNotePool;

            _customNote = selectedNote ?? noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];

            _pluginConfig = pluginConfig;

            _customMultiplayerNoteEventManager = customMultiplayerNoteEventManager;

            _gameNoteController = GetComponent<MultiplayerConnectedPlayerGameNoteController>();
            _customNoteColorNoteVisuals = gameObject.AddComponent<CustomNoteColorNoteVisuals>();

            // The NoteControllers NoteWasCut & NoteWasMissed events aren't fired for Multiplayer Notes ...
            _customMultiplayerNoteEventManager.onGameNoteCutEvent += WasCut;
            _customMultiplayerNoteEventManager.onGameNoteMissEvent += DidFinish;
            _gameNoteController.didInitEvent += Controller_DidInit;
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _gameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            noteMesh.forceRenderingOff = true;
        }

        protected override void DidFinish(NoteController nc) {
            if (nc != this._gameNoteController) return;
            container.transform.SetParent(null);
            switch (nc.noteData.colorType) {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    activePool?.Despawn(container);
                    _customMultiplayerNoteEventManager?.GameNoteDespawnedCallback(_gameNoteController as MultiplayerConnectedPlayerGameNoteController);
                    break;
                default:
                    break;
            }
        }

        protected override void OnDestroy() {
            if (_gameNoteController != null && _customMultiplayerNoteEventManager != null) {
                _customMultiplayerNoteEventManager.onGameNoteCutEvent -= WasCut;
                _customMultiplayerNoteEventManager.onGameNoteMissEvent -= DidFinish;
                _gameNoteController.didInitEvent -= Controller_DidInit;
            }
            if (_customNoteColorNoteVisuals != null) {
                _customNoteColorNoteVisuals.didInitEvent -= Visuals_DidInit;
            }
        }

        protected override void ParentNote(GameObject fakeMesh)
        {
            container.transform.SetParent(noteCube);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            fakeMesh.transform.localRotation = container.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            fakeMesh.transform.rotation = container.transform.rotation = noteCube.parent.parent.rotation;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * _pluginConfig.NoteSize;
            container.transform.localScale = Vector3.one;
        }

        protected override void SpawnThenParent(SiraPrefabContainer.Pool noteModelPool) {
            container = noteModelPool.Spawn();
            activeNote = container.Prefab;
            activePool = noteModelPool;
            SetNoteLayer(activeNote);
            ParentNote(activeNote);
            _customMultiplayerNoteEventManager?.GameNoteSpawnedCallback(_gameNoteController as MultiplayerConnectedPlayerGameNoteController);
        }

    }
}