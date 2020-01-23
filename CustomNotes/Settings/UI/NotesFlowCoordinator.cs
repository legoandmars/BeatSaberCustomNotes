using BeatSaberMarkupLanguage;
using HMUI;
using System;

namespace CustomNotes.Settings.UI
{
    internal class NotesFlowCoordinator : FlowCoordinator
    {
        private NoteListViewController noteListView;
        private NotePreviewViewController notePreviewView;
        private NoteDetailsViewController noteDetailsView;

        public void Awake()
        {
            if (!notePreviewView)
            {
                notePreviewView = BeatSaberUI.CreateViewController<NotePreviewViewController>();
            }

            if (!noteDetailsView)
            {
                noteDetailsView = BeatSaberUI.CreateViewController<NoteDetailsViewController>();
            }

            if (!noteListView)
            {
                noteListView = BeatSaberUI.CreateViewController<NoteListViewController>();
                noteListView.customNoteChanged += noteDetailsView.OnNoteWasChanged;
            }
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            try
            {
                if (firstActivation)
                {
                    title = "Custom Notes";
                    showBackButton = true;
                    ProvideInitialViewControllers(noteListView, noteDetailsView, notePreviewView);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // Dismiss ourselves
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, false);
        }
    }
}
