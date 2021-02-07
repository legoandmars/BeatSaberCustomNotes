using CustomNotes.Data;
using CustomNotes.Providers;
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

        public int SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                if (_selectedNote == 0)
                {
                    _customGameNoteProvider.Priority = -1;
                    _customBombNoteProvider.Priority = -1;
                }
                else
                {
                    _customGameNoteProvider.Priority = 300;
                    _customBombNoteProvider.Priority = CustomNoteObjects[_selectedNote].NoteBomb != null ? 300 : -1;
                }
                
                customNoteSelectionChangedEvent?.Invoke(_selectedNote, CustomNoteObjects[_selectedNote]);
            }
        }

        public event Action<int, CustomNote> customNoteSelectionChangedEvent;

        public IList<CustomNote> CustomNoteObjects { get; private set; }
        public IEnumerable<string> CustomNoteFiles { get; private set; } = Enumerable.Empty<string>();

        private int _selectedNote = 0;
        private readonly CustomGameNoteProvider _customGameNoteProvider;
        private readonly CustomBombNoteProvider _customBombNoteProvider;

        internal NoteAssetLoader(PluginConfig pluginConfig,
            CustomGameNoteProvider customGameNoteProvider,
            CustomBombNoteProvider customBombNoteProvider)
        {
            _pluginConfig = pluginConfig;
            _customGameNoteProvider = customGameNoteProvider;
            _customBombNoteProvider = customBombNoteProvider;
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

        /// <summary>
        /// Returns the CustomNote with hash hash or null if it's not found
        /// </summary>
        /// <param name="hash">The hash to look for</param>
        /// <returns>CustomNote</returns>
        public CustomNote GetNoteByHash(string hash)
        {
            foreach(CustomNote note in CustomNoteObjects)
            {
                if (note.MD5Hash.Equals(hash))
                    return note;
            }
            return null;
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