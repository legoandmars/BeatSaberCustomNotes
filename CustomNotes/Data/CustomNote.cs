using System;
using System.IO;
using UnityEngine;

namespace CustomNotes.Data
{
    public class CustomNote
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public NoteDescriptor NoteDescriptor { get; }
        public GameObject NoteLeft { get; }
        public GameObject NoteRight { get; }
        public GameObject NoteDotLeft { get; }
        public GameObject NoteDotRight { get; }
        public GameObject NoteBomb { get; }

        public CustomNote(string fileName)
        {
            FileName = fileName;

            if (fileName != "DefaultNotes")
            {
                AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.PluginAssetPath, fileName));

                if (AssetBundle != null)
                {
                    GameObject note = AssetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");

                    NoteDescriptor = note?.GetComponent<NoteDescriptor>();
                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    NoteDotLeft = note.transform.Find("NoteDotLeft")?.gameObject;
                    NoteDotRight = note.transform.Find("NoteDotRight")?.gameObject;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                else
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for '{fileName}'!");
                }
            }
            else
            {
                NoteDescriptor = new NoteDescriptor
                {
                    AuthorName = "Beat Saber",
                    NoteName = "Default"
                };
            }
        }

        public CustomNote(byte[] noteObject)
        {
            if (noteObject != null)
            {
                AssetBundle = AssetBundle.LoadFromMemory(noteObject);

                if (AssetBundle != null)
                {
                    GameObject note = AssetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");
                    FileName = note.name;

                    NoteDescriptor = note?.GetComponent<NoteDescriptor>();
                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    NoteDotLeft = note.transform.Find("NoteDotLeft")?.gameObject;
                    NoteDotRight = note.transform.Find("NoteDotRight")?.gameObject;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                else
                {
                    FileName = "Invalid";
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for resource!");
                }
            }
            else
            {
                throw new ArgumentNullException("noteObject cannot be null for the constructor!");
            }
        }
    }
}
