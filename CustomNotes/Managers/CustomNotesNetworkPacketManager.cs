using CustomNotes.Data;
using CustomNotes.Interfaces;
using CustomNotes.Packets;
using CustomNotes.Settings.Utilities;
using MultiplayerExtensions.Packets;
using System;
using System.Collections.Generic;
using Zenject;

namespace CustomNotes.Managers
{
    internal class DummyCustomNotesNetworkPacketManager : ICustomNoteNetworkPacketManager
    {
        private readonly MultiplayerCustomNoteData _defaultData = new MultiplayerCustomNoteData();

        public DummyCustomNotesNetworkPacketManager()
        {
            Logger.log.Info("Disabling CustomNote sync, no MultiplayerExtensions detected.");
        }

        public bool HasDataFromPlayer(IConnectedPlayer connectedPlayer)
        {
            return false;
        }

        public MultiplayerCustomNoteData GetData(IConnectedPlayer connectedPlayer)
        {
            return _defaultData;
        }
    }

    internal class CustomNotesNetworkPacketManager : ICustomNoteNetworkPacketManager, IInitializable, IDisposable
    {
        private readonly PluginConfig _pluginConfig;
        private readonly NoteAssetLoader _noteAssetLoader;

        private readonly PacketManager _packetManager;
        private readonly IMultiplayerSessionManager _sessionManager;

        private readonly MultiplayerCustomNoteData _defaultData = new MultiplayerCustomNoteData();
        private readonly Dictionary<string, MultiplayerCustomNoteData> _customNoteData = new Dictionary<string, MultiplayerCustomNoteData>();

        [Inject]
        public CustomNotesNetworkPacketManager(PluginConfig pluginConfig,
            NoteAssetLoader noteAssetLoader,
            [InjectOptional] PacketManager packetManager,
            [InjectOptional] IMultiplayerSessionManager sessionManager)
        {
            _pluginConfig = pluginConfig;
            _noteAssetLoader = noteAssetLoader;
            _packetManager = packetManager;
            _sessionManager = sessionManager;
        }

        public void Initialize()
        {
            if (_sessionManager == null || _packetManager == null) return;

            Logger.log.Info($"Initializing {nameof(CustomNotesNetworkPacketManager)}");

            _sessionManager.connectedEvent += OnSessionConnectedEvent;
            _sessionManager.playerDisconnectedEvent += OnPlayerDisconnected;
            _packetManager.RegisterCallback<CustomNotesPacket>(HandleCustomNotesPacket);
            _noteAssetLoader.customNoteSelectionChangedEvent += OnCustomNoteSelectionChanged;
        }

        private void OnSessionConnectedEvent()
        {
            Logger.log.Debug("Connected to a new Session!");
            SendUpdatePacket();
        }

        private void OnCustomNoteSelectionChanged(int index, CustomNote note)
        {
            if(_sessionManager.isConnected)
            {
                SendUpdatePacket();
            }
        }

        public void Dispose()
        {
            if (_sessionManager != null)
            {
                _sessionManager.connectedEvent -= OnSessionConnectedEvent;
                _sessionManager.playerDisconnectedEvent -= OnPlayerDisconnected;
            }
            if (_packetManager != null)
            {
                _packetManager.UnregisterCallback<CustomNotesPacket>();
            }
            if(_noteAssetLoader != null)
            {
                _noteAssetLoader.customNoteSelectionChangedEvent -= OnCustomNoteSelectionChanged;
            }
        }
        
        public bool HasDataFromPlayer(IConnectedPlayer connectedPlayer)
        {
            return _customNoteData.ContainsKey(connectedPlayer.userId);
        }

        public MultiplayerCustomNoteData GetData(IConnectedPlayer connectedPlayer)
        {
            return GetCustomNoteData(connectedPlayer) ?? _defaultData;
        }

        private MultiplayerCustomNoteData GetCustomNoteData(IConnectedPlayer connectedPlayer)
        {
            if (_customNoteData.TryGetValue(connectedPlayer.userId, out MultiplayerCustomNoteData data)) return data;
            return null;
        }

        private void HandleCustomNotesPacket(CustomNotesPacket packet, IConnectedPlayer connectedPlayer)
        {
            Logger.log.Info($"Received {nameof(packet)} from {connectedPlayer.userName} ({connectedPlayer.userId}): hash = {packet.NoteHash} , scale = {packet.NoteScale}");
            var data = GetCustomNoteData(connectedPlayer) ?? new MultiplayerCustomNoteData();
            data.UpdateFromPacket(packet);
            _customNoteData[connectedPlayer.userId] = data;
        }

        private void OnPlayerDisconnected(IConnectedPlayer connectedPlayer)
        {
            _customNoteData.Remove(connectedPlayer.userId);
        }

        private void SendUpdatePacket()
        {
            var note = _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
            Logger.log.Debug($"Sending {nameof(CustomNotesPacket)}.");
            _sessionManager.Send(CustomNotesPacket.CreatePacket(note.MD5Hash, _pluginConfig.NoteSize));
        }
    }
}
