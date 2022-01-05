using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zenject;

namespace CustomNotes.Managers
{
    public class NoteAssetLoader : IInitializable, IDisposable
    {
        private readonly PluginConfig _pluginConfig;

        public bool IsLoaded { get; private set; }

        public bool Enabled => _pluginConfig.Enabled;

        public int SelectedNote
        {
            get => _selectedNote;
            set => _selectedNote = value;
        }

        public IList<CustomNote> CustomNoteObjects { get; private set; }
        public IEnumerable<string> CustomNoteFiles { get; private set; } = Enumerable.Empty<string>();

        private int _selectedNote = 0;

        internal NoteAssetLoader(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        /// <summary>
        /// Load all CustomNotes
        /// </summary>
        public void Initialize()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);

                IEnumerable<string> noteFilter = new List<string> { "*.bloq", "*.note", };
                CustomNoteFiles = Utils.GetFileNames(Plugin.PluginAssetPath, noteFilter, SearchOption.AllDirectories, true);
                Logger.log.Debug($"{CustomNoteFiles.Count()} external note(s) found.");

                CustomNoteObjects = LoadCustomNotes(CustomNoteFiles);
                Logger.log.Debug($"{CustomNoteObjects.Count} total note(s) loaded.");

                SelectedNote = 0;
                if (_pluginConfig.LastNote != null)
                {
                    int numberOfNotes = CustomNoteObjects.Count;
                    for (int i = 0; i < numberOfNotes; i++)
                    {
                        if (CustomNoteObjects[i].FileName == _pluginConfig.LastNote)
                        {
                            SelectedNote = i;
                            break;
                        }
                    }
                }
                IsLoaded = true;
            }
        }

        /// <summary>
        /// Reload all CustomNotes
        /// </summary>
        internal void Reload()
        {
            Logger.log.Debug("Reloading the NoteAssetLoader");

            Dispose();
            Initialize();
        }

        /// <summary>
        /// Clear all loaded CustomNotes
        /// </summary>
        public void Dispose()
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

        private IList<CustomNote> LoadCustomNotes(IEnumerable<string> customNoteFiles)
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