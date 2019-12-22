using CustomNotes.Data;
using CustomNotes.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomNotes.Utilities
{
    public class NoteAssetLoader
    {
        public static bool IsLoaded { get; private set; }
        public static int SelectedNote { get; internal set; } = 0;
        public static IEnumerable<CustomNote> CustomNoteObjects { get; private set; }
        public static IEnumerable<string> CustomNoteFiles { get; private set; } = Enumerable.Empty<string>();

        internal static void LoadCustomNotes()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);

                IEnumerable<string> noteFilter = new List<string> { "*.bloq", "*.note", };
                CustomNoteFiles = Utils.GetFileNames(Plugin.PluginAssetPath, noteFilter, SearchOption.TopDirectoryOnly);
                Logger.log.Debug($"{CustomNoteFiles.Count()} note(s) found.");

                CustomNoteObjects = LoadCustomNotes(CustomNoteFiles);
                Logger.log.Debug($"{CustomNoteObjects.Count()} note(s) loaded.");

                if (Configuration.CurrentlySelectedNote != null)
                {
                    int numberOfNotes = CustomNoteObjects.Count();
                    for (int i = 0; i < numberOfNotes; i++)
                    {
                        if (CustomNoteObjects.ElementAt(i).FileName == Configuration.CurrentlySelectedNote)
                        {
                            SelectedNote = i;
                        }
                    }
                }

                IsLoaded = true;
            }
        }

        private static IEnumerable<CustomNote> LoadCustomNotes(IEnumerable<string> customNoteFiles)
        {
            IList<CustomNote> customNotes = new List<CustomNote>
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
                        customNotes.Add(newNote);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log.Warn($"Failed to Load Custom Note with name '{customNoteFile}'.");
                    Logger.log.Warn(ex);
                }
            }

            return customNotes;
        }
    }
}
