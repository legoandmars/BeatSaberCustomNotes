using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes.Utilities
{
    public static class NoteAssetLoader
    {
        public static bool IsLoaded { get; private set; }

        internal static int selectedNote = 0;
        internal static List<string> customNoteFiles = new List<string>();
        internal static CustomNote[] customNotes;

        internal static bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid;

            try
            {
                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }

        internal static void LoadCustomNotes()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);
                List<string> files = Directory.GetFiles(Plugin.PluginAssetPath,
                    "*.note", SearchOption.TopDirectoryOnly).Concat(Directory.GetFiles(Plugin.PluginAssetPath,
                    "*.bloq", SearchOption.TopDirectoryOnly)).ToList();

                foreach (string file in files)
                {
                    customNoteFiles.Add(file.Split('\\', '/').Last());
                }

                Logger.Log($"Found {customNoteFiles.Count} note(s)", LogLevel.Debug);
                List<CustomNote> loadedNotes = new List<CustomNote>
                {
                    new CustomNote("DefaultNotes"),
                };

                foreach (string customNoteFile in customNoteFiles)
                {
                    try
                    {
                        CustomNote newNote = new CustomNote(customNoteFile);
                        if (newNote.AssetBundle != null)
                        {
                            loadedNotes.Add(newNote);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to Load Custom Note with name \"{customNoteFile}\"", LogLevel.Warning);
                        Logger.Log(ex, LogLevel.Warning);
                    }
                }

                customNotes = loadedNotes.ToArray();

                if (Configuration.CurrentlySelectedNote != null)
                {
                    int currentNoteCount = 0;
                    foreach (CustomNote customNote in customNotes)
                    {
                        if (customNote.FileName == Configuration.CurrentlySelectedNote)
                        {
                            selectedNote = currentNoteCount;
                        }

                        currentNoteCount++;
                    }
                }

                IsLoaded = true;
            }
        }

        private static void InjectNotes(BeatmapObjectSpawnController spawnContoller, NoteController noteController) { }
    }
}
