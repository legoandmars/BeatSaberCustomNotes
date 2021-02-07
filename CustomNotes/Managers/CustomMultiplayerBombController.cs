using CustomNotes.Data;
using CustomNotes.Packets;
using CustomNotes.Providers;
using CustomNotes.Settings.Utilities;
using SiraUtil.Objects;
using System;
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

            string id = connectedPlayerNotePoolProvider.GetPoolIDForPlayer(connectedPlayer);

            if (id.Equals(CustomNotesPacket.DEFAULT_NOTES))
            {
                return;
            }

            bombPool = Container.TryResolveId<SiraPrefabContainer.Pool>($"cn.multi.{id}.bomb");

            _customNote = Container.TryResolveId<CustomNote>($"cn.multi.{id}.note");

            if (bombPool == null)
            {
                return;
            }

            try
            {
                _noteSize = Container.ResolveId<float>($"cn.multi.{connectedPlayer.userId}.scale");
            }
            catch (Exception ex)
            {
                Logger.log.Error($" ({nameof(CustomMultiplayerBombController)}) This shouldn't happen: {ex.Message}");
            }

            _bombNoteController = GetComponent<MultiplayerConnectedPlayerBombNoteController>();
            _noteMovement = GetComponent<NoteMovement>();
            _bombNoteController.didInitEvent += Controller_Init;
            _noteMovement.noteDidFinishJumpEvent += DidFinish;
            bombMesh = gameObject.transform.Find("Mesh");

            MeshRenderer bm = GetComponentInChildren<MeshRenderer>();
            bm.enabled = false;
        }
    }
}
