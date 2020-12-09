using CustomNotes.Utilities;
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
        public string ErrorMessage { get; } = string.Empty;

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
                    Descriptor.Icon = Descriptor.Icon ?? Utils.GetDefaultCustomIcon();

                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    Transform NoteDotLeftTransform = note.transform.Find("NoteDotLeft");
                    Transform NoteDotRightTransform = note.transform.Find("NoteDotRight");
                    NoteDotLeft = NoteDotLeftTransform != null ? NoteDotLeftTransform.gameObject : NoteLeft;
                    NoteDotRight = NoteDotRightTransform != null ? NoteDotRightTransform.gameObject : NoteRight;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                catch (Exception ex)
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for '{FileName}'!");
                    Logger.log.Warn(ex);

                    Descriptor = new NoteDescriptor
                    {
                        NoteName = "Invalid Note (Delete it!)",
                        AuthorName = FileName,
                        Icon = Utils.GetErrorIcon()
                    };

                    ErrorMessage = $"File: '{fileName}'" +
                                    "\n\nThis file failed to load." +
                                    "\n\nThis may have been caused by having duplicated files," +
                                    " another note with the same name already exists or that the custom note is simply just broken." +
                                    "\n\nThe best thing is probably just to delete it!";

                    FileName = "DefaultNotes";
                }
            }
            else
            {
                Descriptor = new NoteDescriptor
                {
                    AuthorName = "Beat Saber",
                    NoteName = "Default",
                    Description = "This is the default notes. (No preview available)",
                    Icon = Utils.GetDefaultIcon()
                };
            }
        }

        public CustomNote(byte[] noteObject, string name)
        {
            if (noteObject != null)
            {
                try
                {
                    AssetBundle = AssetBundle.LoadFromMemory(noteObject);
                    GameObject note = AssetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");
                    FileName = $@"internalResource\{name}";

                    Descriptor = note.GetComponent<NoteDescriptor>();
                    Descriptor.Icon = Descriptor.Icon ?? Utils.GetDefaultCustomIcon();

                    NoteLeft = note.transform.Find("NoteLeft").gameObject;
                    NoteRight = note.transform.Find("NoteRight").gameObject;
                    NoteDotLeft = note.transform.Find("NoteDotLeft")?.gameObject;
                    NoteDotRight = note.transform.Find("NoteDotRight")?.gameObject;
                    NoteBomb = note.transform.Find("NoteBomb")?.gameObject;
                }
                catch (Exception ex)
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle from a resource!");
                    Logger.log.Warn(ex);

                    Descriptor = new NoteDescriptor
                    {
                        NoteName = "Internal Error (Report it!)",
                        AuthorName = FileName,
                        Icon = Utils.GetErrorIcon()
                    };
                    ErrorMessage = $@"File: 'internalResource\\{name}'" +
                                    "\n\nAn internal asset has failed to load." +
                                    "\n\nThis shouldn't have happened and should be reported!" +
                                    " Remember to include the log related to this incident.";

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
