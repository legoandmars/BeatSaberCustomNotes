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
    internal class ConnectedPlayerNotePoolProvider : IInitializable, IDisposable
    {
        private DiContainer _container;
        private PluginConfig _pluginConfig;
        private NoteAssetLoader _noteAssetLoader;
        private ICustomNoteNetworkPacketManager _noteHashPacketManager;

        private readonly CustomMultiplayerNoteProvider _customMultiplayerNoteProvider;
        private readonly CustomMultiplayerBombProvider _customMultiplayerBombProvider;

        private IMultiplayerSessionManager _multiplayerSessionManager;

        private Dictionary<IConnectedPlayer, string> _connectedPlayerPoolIDs;

        private static MD5 _staticMd5Hasher = MD5.Create();

        [Inject]
        internal ConnectedPlayerNotePoolProvider(DiContainer Container,
            PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            ICustomNoteNetworkPacketManager noteHashPacketManager,
            CustomMultiplayerNoteProvider customMultiplayerNoteProvider,
            CustomMultiplayerBombProvider customMultiplayerBombProvider,
            [InjectOptional] IMultiplayerSessionManager multiplayerSessionManager)
        {
            _container = Container;
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _noteHashPacketManager = noteHashPacketManager;

            _customMultiplayerNoteProvider = customMultiplayerNoteProvider;
            _customMultiplayerBombProvider = customMultiplayerBombProvider;

            _multiplayerSessionManager = multiplayerSessionManager;
        }

        public void Initialize()
        {
            if (_multiplayerSessionManager == null) return;

            _connectedPlayerPoolIDs = new Dictionary<IConnectedPlayer, string>();

            if (_pluginConfig.OtherPlayerMultiplayerNotes)
            {
                _customMultiplayerNoteProvider.Priority = 300;
                _customMultiplayerBombProvider.Priority = 300;
            }
            else
            {
                _customMultiplayerNoteProvider.Priority = -1;
                _customMultiplayerBombProvider.Priority = -1;
                return;
            }

            foreach (IConnectedPlayer connectedPlayer in _multiplayerSessionManager.connectedPlayers)
            {
                CreateNotePoolsForPlayer(connectedPlayer);
            }
        }

        private void CreateNotePoolsForPlayer(IConnectedPlayer connectedPlayer)
        {
            Logger.log.Debug($"Creating note pools for player \"{connectedPlayer.userName}\" - \"{connectedPlayer.userId}\" ...");

            CustomNote note = null;
            float noteScale = 1f;
            if (_pluginConfig.SyncNotesInMultiplayer && _noteHashPacketManager.HasDataFromPlayer(connectedPlayer))
            {
                Logger.log.Debug($"Preparing note from hash for player \"{connectedPlayer.userName}\"");

                var multiplayerNoteData = _noteHashPacketManager.GetData(connectedPlayer);

                note = _noteAssetLoader.GetNoteByHash(multiplayerNoteData.NoteHash);
                noteScale = multiplayerNoteData.NoteScale;
            }

            if (note == null && _pluginConfig.RandomMultiplayerNotes)
            {
                System.Random rng;

                if (_pluginConfig.RandomnessIsConsistentPerPlayer)
                {
                    // Set the Random seed based on player ID -> same random number every time for the same player
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
            else if(note == null)
            {
                Logger.log.Debug("using same notes as local player ...");
                note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
            }

            _container.Bind<float>().WithId($"cn.multi.{connectedPlayer.userId}.scale").FromInstance(noteScale);

            foreach (KeyValuePair<IConnectedPlayer, string> entry in _connectedPlayerPoolIDs)
            {
                if (entry.Value.Equals(note.MD5Hash))
                {
                    Logger.log.Debug($"Reusing old note pool for \"{note.FileName}\"");
                    _connectedPlayerPoolIDs.Add(connectedPlayer, note.MD5Hash);
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

            _connectedPlayerPoolIDs.Add(connectedPlayer, note.MD5Hash);
        }

        public string GetPoolIDForPlayer(IConnectedPlayer connectedPlayer)
        {
            if (_connectedPlayerPoolIDs.TryGetValue(connectedPlayer, out string id))
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

        }
    }
}
