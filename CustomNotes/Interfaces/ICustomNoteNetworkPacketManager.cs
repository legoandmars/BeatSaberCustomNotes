using CustomNotes.Data;

namespace CustomNotes.Interfaces
{
    internal interface ICustomNoteNetworkPacketManager
    {
        bool HasDataFromPlayer(IConnectedPlayer connectedPlayer);

        MultiplayerCustomNoteData GetData(IConnectedPlayer connectedPlayer);
    }
}
