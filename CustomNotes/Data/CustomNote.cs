using System;
using System.IO;
using UnityEngine;

namespace CustomNotes.Data
{
    public class CustomNote
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public NoteDescriptor Descriptor { get; }
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
                try
                {
                    AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.PluginAssetPath, fileName));
                    GameObject note = AssetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");

                    Descriptor = note.GetComponent<NoteDescriptor>();
                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    NoteDotLeft = note.transform.Find("NoteDotLeft")?.gameObject;
                    NoteDotRight = note.transform.Find("NoteDotRight")?.gameObject;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                catch
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for '{FileName}'!");

                    Descriptor = new NoteDescriptor
                    {
                        NoteName = "Invalid Note (Delete it!)",
                        AuthorName = FileName,
                    };

                    FileName = "DefaultNotes";
                }
            }
            else
            {
                Descriptor = new NoteDescriptor
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
                try
                {
                    AssetBundle = AssetBundle.LoadFromMemory(noteObject);
                    GameObject note = AssetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");
                    FileName = note.name;

                    Descriptor = note.GetComponent<NoteDescriptor>();
                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    NoteDotLeft = note.transform.Find("NoteDotLeft")?.gameObject;
                    NoteDotRight = note.transform.Find("NoteDotRight")?.gameObject;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                catch (Exception ex)
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for resource!");
                    Logger.log.Warn(ex);

                    Descriptor = new NoteDescriptor
                    {
                        NoteName = "Internal Error (Report it!)",
                        AuthorName = FileName,
                    };

                    FileName = "DefaultNotes";
                }
            }
            else
            {
                throw new ArgumentNullException("noteObject cannot be null for the constructor!");
            }
        }

        public void Destroy()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
            }
            else
            {
                UnityEngine.Object.Destroy(Descriptor);
            }
        }
    }
}
