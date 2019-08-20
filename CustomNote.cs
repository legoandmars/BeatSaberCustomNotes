using System.IO;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes
{
    public class CustomNote
    {
        public string FileName { get; private set; }
        public AssetBundle AssetBundle { get; private set; }
        public NoteDescriptor NoteDescriptor { get; private set; }
        public GameObject NoteLeft { get; private set; }
        public GameObject NoteRight { get; private set; }
        public GameObject NoteDotLeft { get; private set; }
        public GameObject NoteDotRight { get; private set; }
        public GameObject NoteBomb { get; private set; }

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
                    Logger.Log("Something went wrong getting the AssetBundle!", LogLevel.Warning);
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
    }
}
