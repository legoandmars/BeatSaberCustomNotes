using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using System;

namespace CustomNotes.Settings.UI
{
    internal class NotesFlowCoordinator : FlowCoordinator
    {
        private NotesListView notesListView;
        private NotePreviewView notesPreviewView;

        public void Awake()
        {
            if (notesListView == null)
            {
                notesListView = BeatSaberUI.CreateViewController<NotesListView>();
                notesPreviewView = BeatSaberUI.CreateViewController<NotePreviewView>();
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
                    ProvideInitialViewControllers(notesListView, null, notesPreviewView);
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
