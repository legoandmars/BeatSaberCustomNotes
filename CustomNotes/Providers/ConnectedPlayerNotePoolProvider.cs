using CustomNotes.Data;
using CustomNotes.Interfaces;
using CustomNotes.Managers;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Objects;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Zenject;

namespace CustomNotes.Providers
{
    internal class ConnectedPlayerNotePoolProvider : IDisposable
    {

        private MultiplayerController _multiplayerController;
        private IMultiplayerSessionManager _multiplayerSessionManager;
        private DiContainer _container;
        private PluginConfig _pluginConfig;
        private NoteAssetLoader _noteAssetLoader;
        private INoteHashPacketManager _noteHashPacketManager;

        private Dictionary<IConnectedPlayer, string> _connectedPlayerPoolID;

        private static MD5 _staticMd5Hasher = MD5.Create();

        [Inject]
        internal ConnectedPlayerNotePoolProvider(DiContainer Container,
            PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            INoteHashPacketManager noteHashPacketManager,
            [InjectOptional] MultiplayerController multiplayerController,
            [InjectOptional] IMultiplayerSessionManager multiplayerSessionManager)
        {
            _multiplayerController = multiplayerController;
            _multiplayerSessionManager = multiplayerSessionManager;
            _container = Container;
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _noteHashPacketManager = noteHashPacketManager;

            if (_multiplayerController == null) return;

            _connectedPlayerPoolID = new Dictionary<IConnectedPlayer, string>();

            _multiplayerSessionManager.playerConnectedEvent += HandlePlayerConnectedEventChecked;

            foreach (IConnectedPlayer connectedPlayer in _multiplayerSessionManager.connectedPlayers)
            {
                Logger.log.Debug($"Player already connected: \"{connectedPlayer.userName}\" - \"{connectedPlayer.userId}\"");
                HandlePlayerConnectedEvent(connectedPlayer);
            }
        }

        private void HandlePlayerConnectedEventChecked(IConnectedPlayer connectedPlayer)
        {
            if (_multiplayerController.state != MultiplayerController.State.WaitingForPlayers || _multiplayerController.state != MultiplayerController.State.CheckingLobbyState) return;
            HandlePlayerConnectedEvent(connectedPlayer);
        }

        private void HandlePlayerConnectedEvent(IConnectedPlayer connectedPlayer)
        {
            Logger.log.Debug($"Creating note pools for player \"{connectedPlayer.userName}\" - \"{connectedPlayer.userId}\" ...");

            CustomNote note = null;
            if (_pluginConfig.SyncNotesInMultiplayer && _noteHashPacketManager.HasHashFromPlayer(connectedPlayer))
            {
                Logger.log.Debug($"Preparing note from hash for player \"{connectedPlayer.userName}\"");

                note = _noteAssetLoader.GetNoteByHash(_noteHashPacketManager.GetHashFromPlayer(connectedPlayer));
            }

            if (note == null)
            {
                System.Random rng;

                if (_pluginConfig.RandomnessIsConsistentPerPlayer)
                {
                    // Set the Random seed based on player ID -> same random number every time for the same player, hashing is probably unnecessary but eh ¯\_(ツ)_/¯
                    var hashed = _staticMd5Hasher.ComputeHash(Encoding.UTF8.GetBytes(connectedPlayer?.userId));
                    var ivalue = BitConverter.ToInt32(hashed, 0);
                    rng = new System.Random(ivalue);
                }
                else
                {
                    rng = new System.Random();
                }

                note = _pluginConfig.RandomMultiplayerNotes ? _noteAssetLoader.CustomNoteObjects[rng.Next(1, _noteAssetLoader.CustomNoteObjects.Count)] : _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
            }


            foreach (KeyValuePair<IConnectedPlayer, string> entry in _connectedPlayerPoolID)
            {
                if (entry.Value.Equals(note.MD5Hash))
                {
                    Logger.log.Debug($"Reusing old note pool for \"{note.FileName}\"");
                    _connectedPlayerPoolID.Add(connectedPlayer, note.MD5Hash);
                    return;
                }

            }

            Logger.log.Debug($"Creating new pool for note \"{note.FileName}\" with hash \"{note.MD5Hash}\"");

            MaterialSwapper.GetMaterials();
            MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteLeft);
            MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteRight);
            MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotLeft);
            MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotRight);
            MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
            Utils.AddMaterialPropertyBlockController(note.NoteLeft);
            Utils.AddMaterialPropertyBlockController(note.NoteRight);
            Utils.AddMaterialPropertyBlockController(note.NoteDotLeft);
            Utils.AddMaterialPropertyBlockController(note.NoteDotRight);

            _container.Bind<CustomNote>().WithId($"cn.multi.{note.MD5Hash}.note").FromInstance(note);
            _container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId($"cn.multi.{note.MD5Hash}.left.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteLeft));
            _container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId($"cn.multi.{note.MD5Hash}.right.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteRight));
            if (note.NoteDotLeft != null)
            {
                _container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId($"cn.multi.{note.MD5Hash}.left.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
            }
            if (note.NoteDotRight != null)
            {
                _container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId($"cn.multi.{note.MD5Hash}.right.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));
            }
            if (note.NoteBomb)
            {
                _container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId($"cn.multi.{note.MD5Hash}.bomb").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
            }

            _connectedPlayerPoolID.Add(connectedPlayer, note.MD5Hash);
        }

        internal string GetPoolIDForPlayer(IConnectedPlayer connectedPlayer)
        {
            if (_connectedPlayerPoolID.TryGetValue(connectedPlayer, out string id))
                return id;
            return string.Empty;
        }

        private SiraPrefabContainer NotePrefabContainer(GameObject initialPrefab)
        {
            var prefab = new GameObject("CustomNotes_Multi" + initialPrefab.name).AddComponent<SiraPrefabContainer>();
            prefab.Prefab = initialPrefab;
            return prefab;
        }

        public void Dispose()
        {
            if(_multiplayerSessionManager != null)
            {
                _multiplayerSessionManager.playerConnectedEvent -= HandlePlayerConnectedEventChecked;
            }
        }
    }
}
