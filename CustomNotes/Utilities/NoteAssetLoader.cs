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
        public static IList<CustomNote> CustomNoteObjects { get; private set; }
        public static IEnumerable<string> CustomNoteFiles { get; private set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Load all CustomNotes
        /// </summary>
        internal static void Load()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);

                IEnumerable<string> noteFilter = new List<string> { "*.bloq", "*.note", };
                CustomNoteFiles = Utils.GetFileNames(Plugin.PluginAssetPath, noteFilter, SearchOption.AllDirectories, true);
                Logger.log.Debug($"{CustomNoteFiles.Count()} external note(s) found.");

                CustomNoteObjects = LoadCustomNotes(CustomNoteFiles);
                Logger.log.Debug($"{CustomNoteObjects.Count} total note(s) loaded.");

                /*if (Configuration.CurrentlySelectedNote != null)
                {
                    int numberOfNotes = CustomNoteObjects.Count;
                    for (int i = 0; i < numberOfNotes; i++)
                    {
                        if (CustomNoteObjects[i].FileName == Configuration.CurrentlySelectedNote)
                        {
                            SelectedNote = i;
                            break;
                        }
                    }
                }*/

                IsLoaded = true;
            }
        }

        /// <summary>
        /// Reload all CustomNotes
        /// </summary>
        internal static void Reload()
        {
            Logger.log.Debug("Reloading the NoteAssetLoader");

            Clear();
            Load();
        }

        /// <summary>
        /// Clear all loaded CustomNotes
        /// </summary>
        internal static void Clear()
        {
            int numberOfObjects = CustomNoteObjects.Count;
            for (int i = 0; i < numberOfObjects; i++)
            {
                CustomNoteObjects[i].Destroy();
                CustomNoteObjects[i] = null;
            }
            IsLoaded = false;
            SelectedNote = 0;
            CustomNoteObjects = new List<CustomNote>();
            CustomNoteFiles = Enumerable.Empty<string>();
        }

        private static IList<CustomNote> LoadCustomNotes(IEnumerable<string> customNoteFiles)
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
                    if (newNote != null)
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