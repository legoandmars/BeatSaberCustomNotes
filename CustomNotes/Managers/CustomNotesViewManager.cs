using CustomNotes.Data;
using CustomNotes.Settings;
using CustomNotes.Settings.UI;
using System;
using Zenject;

namespace CustomNotes.Managers
{
    class CustomNotesViewManager : IInitializable, IDisposable
    {
        private NoteListViewController _noteListViewController;
        private NoteDetailsViewController _noteDetailsViewController;
        private NoteModifierViewController _noteModifierViewController;

        public CustomNotesViewManager(NoteListViewController noteListViewController, NoteDetailsViewController noteDetailsViewController, NoteModifierViewController noteModifierViewController)
        {
            _noteListViewController = noteListViewController;
            _noteDetailsViewController = noteDetailsViewController;
            _noteModifierViewController = noteModifierViewController;
        }

        public void Initialize()
        {
            _noteListViewController.customNoteChanged += NoteListViewController_CustomNoteChanged;
            _noteListViewController.customNotesReloaded += NoteListViewController_CustomNotesReloaded;
            _noteDetailsViewController.noteSizeChanged += NoteDetailsViewController_NoteSizeChanged;
            _noteDetailsViewController.hmdOnlyChanged += NoteDetailsViewController_HmdOnlyChanged;
            _noteModifierViewController.noteSizeChanged += NoteModifierViewController_NoteSizeChanged;
            _noteModifierViewController.hmdOnlyChanged += NoteModifierViewController_HmdOnlyChanged;
        }

        public void Dispose()
        {
            _noteListViewController.customNoteChanged -= NoteListViewController_CustomNoteChanged;
            _noteListViewController.customNotesReloaded -= NoteListViewController_CustomNotesReloaded;
            _noteDetailsViewController.noteSizeChanged -= NoteDetailsViewController_NoteSizeChanged;
            _noteDetailsViewController.hmdOnlyChanged -= NoteDetailsViewController_HmdOnlyChanged;
            _noteModifierViewController.noteSizeChanged -= NoteModifierViewController_NoteSizeChanged;
            _noteModifierViewController.hmdOnlyChanged -= NoteModifierViewController_HmdOnlyChanged;
        }

        private void NoteListViewController_CustomNoteChanged(CustomNote customNote)
        {
            _noteModifierViewController.OnNoteListSelected(customNote);
        }

        private void NoteListViewController_CustomNotesReloaded()
        {
            _noteModifierViewController.OnNotesReloaded();
        }

        private void NoteDetailsViewController_NoteSizeChanged(float noteSize)
        {
            _noteModifierViewController.OnNoteSizeChanged(noteSize);
        }

        private void NoteDetailsViewController_HmdOnlyChanged(bool hmdOnly)
        {
            _noteModifierViewController.OnHmdOnlyChanged(hmdOnly);
        }

        private void NoteModifierViewController_NoteSizeChanged(float noteSize)
        {
            _noteDetailsViewController.OnNoteSizeChanged(noteSize);
        }

        private void NoteModifierViewController_HmdOnlyChanged(bool hmdOnly)
        {
            _noteDetailsViewController.OnHmdOnlyChanged(hmdOnly);
        }
    }
}
