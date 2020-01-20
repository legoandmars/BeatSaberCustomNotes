using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
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
            if (noteListView == null)
            {
                noteListView = BeatSaberUI.CreateViewController<NoteListViewController>();
                notePreviewView = BeatSaberUI.CreateViewController<NotePreviewViewController>();
                noteDetailsView = BeatSaberUI.CreateViewController<NoteDetailsViewController>();

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
            MainFlowCoordinator mainFlow = BeatSaberUI.MainFlowCoordinator;
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
