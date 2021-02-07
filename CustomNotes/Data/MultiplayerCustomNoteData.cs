using CustomNotes.Packets;

namespace CustomNotes.Data
{
    class MultiplayerCustomNoteData
    {

        public string NoteHash { get; private set; } = CustomNotesPacket.DEFAULT_NOTES;
        public float NoteScale { get; private set; } = 1f;

        public MultiplayerCustomNoteData(string noteHash = CustomNotesPacket.DEFAULT_NOTES, float noteScale = 1f) {
            NoteHash = noteHash;
            NoteScale = noteScale;
        }

        public void UpdateFromPacket(CustomNotesPacket packet) {
            NoteHash = packet.NoteHash;
            NoteScale = packet.NoteScale;
        }

    }
}
