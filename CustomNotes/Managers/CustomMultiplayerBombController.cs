using CustomNotes.Data;
using CustomNotes.Providers;
using CustomNotes.Settings.Utilities;
using SiraUtil.Objects;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    class CustomMultiplayerBombController : CustomBombControllerBase
    {
        [Inject]
        internal void Init(DiContainer Container,
            PluginConfig pluginConfig,
            ConnectedPlayerNotePoolProvider connectedPlayerNotePoolProvider,
            IConnectedPlayer connectedPlayer)
        {
            _pluginConfig = pluginConfig;

            Logger.log.Debug($"IConnectedPlayer injected: {connectedPlayer?.userName}");

            string id = connectedPlayerNotePoolProvider.GetPoolIDForPlayer(connectedPlayer);

            bombPool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.bomb");

            _customNote = Container.TryResolveId<CustomNote>($"cn.multi.{id}.note");

            if (bombPool == null) {
                return;
            }

            _bombNoteController = GetComponent<MultiplayerConnectedPlayerBombNoteController>();
            _noteMovement = GetComponent<NoteMovement>();
            _bombNoteController.didInitEvent += Controller_Init;
            _noteMovement.noteDidFinishJumpEvent += DidFinish;
            bombMesh = gameObject.transform.Find("Mesh");

            MeshRenderer bm = GetComponentInChildren<MeshRenderer>();
            bm.enabled = false;
        }

        protected override void DidFinish() {
            container.transform.SetParent(null);
            bombPool.Despawn(container);
            Logger.log.Debug("Bomb despawned!");
        }

        protected override void Controller_Init(NoteController noteController) {
            SpawnThenParent(bombPool);
            Logger.log.Debug("Bomb initialized!");
        }
    }
}
