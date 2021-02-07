using LiteNetLib.Utils;
using UnityEngine;

namespace CustomNotes.Packets
{
    class CustomNotesPacket : INetSerializable, IPoolablePacket
    {
        public const int MAX_LENGTH = 32; // MD5 Hash length as a hexadecimal string
        public const string DEFAULT_NOTES = "_hello_i_am_using_default_notes_";
        public const float MIN_NOTE_SIZE = 0.4f;
        public const float MAX_NOTE_SIZE = 2f;

        private string _noteHash = DEFAULT_NOTES;
        public string NoteHash
        {
            get => _noteHash;
            set
            {
                if (value.Length < MAX_LENGTH)
                {
                    _noteHash = value.PadRight(MAX_LENGTH);
                }
                else if (value.Length > MAX_LENGTH)
                {
                    _noteHash = value.Truncate(MAX_LENGTH);
                }
                else
                {
                    _noteHash = value;
                }
            }
        }

        private float _noteScale = 1f;
        public float NoteScale
        {
            get => _noteScale;
            set
            {
                _noteScale = Mathf.Max(Mathf.Min(value, MAX_NOTE_SIZE), MIN_NOTE_SIZE);
            }
        }

        public void Release() => ThreadStaticPacketPool<CustomNotesPacket>.pool.Release(this);

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NoteHash, MAX_LENGTH);
            writer.Put(NoteScale);
        }

        public void Deserialize(NetDataReader reader)
        {
            NoteHash = reader.GetString(MAX_LENGTH);
            NoteScale = reader.GetFloat();
        }

        public CustomNotesPacket Init(string hash, float noteScale = 1f)
        {
            NoteHash = hash;
            NoteScale = noteScale;

            return this;
        }

        public static CustomNotesPacket CreatePacket(string hash, float noteScale = 1f) => ThreadStaticPacketPool<CustomNotesPacket>.pool.Obtain().Init(hash, noteScale);
    }
}
