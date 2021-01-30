using CustomNotes.Interfaces;
using System;

namespace CustomNotes.Managers
{
    internal class DummyNoteHashPacketManager : INoteHashPacketManager
    {
        public string GetHashFromPlayer(IConnectedPlayer connectedPlayer)
        {
            return string.Empty;
        }
        public bool HasHashFromPlayer(IConnectedPlayer connectedPlayer)
        {
            return false;
        }
    }

    internal class NoteHashPacketManager : INoteHashPacketManager
    {
        public string GetHashFromPlayer(IConnectedPlayer connectedPlayer) => throw new NotImplementedException();
        public bool HasHashFromPlayer(IConnectedPlayer connectedPlayer) => throw new NotImplementedException();
    }
}
