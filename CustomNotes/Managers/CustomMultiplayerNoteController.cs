using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using CustomNotes.Data;
using CustomNotes.Overrides;
using CustomNotes.Settings.Utilities;
using CustomNotes.Providers;

namespace CustomNotes.Managers
{
    public class CustomMultiplayerNoteController : CustomNoteControllerBase
    {

        [Inject]
        private CustomMultiplayerNoteEventManager _customMultiplayerNoteEventManager;

        [Inject]
        internal void Init(DiContainer Container, 
            PluginConfig pluginConfig,
            ConnectedPlayerNotePoolProvider connectedPlayerNotePoolProvider,
            CustomMultiplayerNoteEventManager customMultiplayerNoteEventManager,
            IConnectedPlayer connectedPlayer)
        {

            Logger.log.Debug($"IConnectedPlayer injected: {connectedPlayer?.userName}");

            string id = connectedPlayerNotePoolProvider.GetPoolIDForPlayer(connectedPlayer);

            _leftArrowNotePool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.left.arrow");
            _rightArrowNotePool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.right.arrow");
            _leftDotNotePool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.left.dot") ?? _leftArrowNotePool;
            _rightDotNotePool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.right.dot") ?? _rightArrowNotePool;

            _customNote = Container.TryResolveId<CustomNote>($"cn.multi.{id}.note");

            if(_leftArrowNotePool == null) {
                return;
            }

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
            fakeMesh.transform.rotation = fakeMesh.transform.localRotation = container.transform.rotation = Quaternion.Euler(0f,0f,0f);
            container.transform.localRotation = noteCube.parent.parent.rotation;
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