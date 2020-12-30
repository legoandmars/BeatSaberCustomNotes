using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using SiraUtil.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    class CustomMultiplayerConnectedPlayerBombNoteController : CustomBombControllerBase
    {
        [Inject]
        internal void Init(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            [Inject(Id = "cn.multi.bomb")] SiraPrefabContainer.Pool bombContainerPool,
            [Inject(Id = "cn.multi.note")] CustomNote selectedNote) {
            _pluginConfig = pluginConfig;

            _customNote = selectedNote ?? noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];
            _bombNoteController = GetComponent<MultiplayerConnectedPlayerBombNoteController>();
            _noteMovement = GetComponent<NoteMovement>();
            _bombNoteController.didInitEvent += Controller_Init;
            _noteMovement.noteDidFinishJumpEvent += DidFinish;
            bombMesh = gameObject.transform.Find("Mesh");
            bombPool = bombContainerPool;

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
