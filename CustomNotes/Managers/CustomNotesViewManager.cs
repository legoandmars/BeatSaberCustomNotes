using CustomNotes.Data;
using CustomNotes.Settings.UI;
using System;
using Zenject;

namespace CustomNotes.Managers
{
    class CustomNotesViewManager : IInitializable, IDisposable
    {
        private NoteListViewController _noteListViewController;
        private NoteModifierViewController _noteModifierViewController;

        public CustomNotesViewManager(NoteListViewController noteListViewController, NoteModifierViewController noteModifierViewController)
        {
            _noteListViewController = noteListViewController;
            _noteModifierViewController = noteModifierViewController;
        }

        public void Initialize()
        {
            _noteListViewController.customNoteChanged += NoteListViewController_CustomNoteChanged;
            _noteListViewController.customNotesReloaded += NoteListViewController_CustomNotesReloaded;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void NoteListViewController_CustomNoteChanged(CustomNote customNote)
        {
            _noteModifierViewController.OnNoteListSelected(customNote);
        }

        private void NoteListViewController_CustomNotesReloaded()
        {
            _noteModifierViewController.OnNotesReloaded();
        }
    }
}
