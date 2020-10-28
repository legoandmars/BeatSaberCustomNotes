using HMUI;
using Zenject;
using BeatSaberMarkupLanguage;

namespace CustomNotes.Settings.UI
{
    internal class NotesFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlow;
        private NoteListViewController _noteListView;
        private NoteDetailsViewController _noteDetailsView;
        private NotePreviewViewController _notePreviewView;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlow, NoteListViewController noteListView, NoteDetailsViewController noteDetailsView, NotePreviewViewController notePreviewView)
        {
            _mainFlow = mainFlow;
            _noteListView = noteListView;
            _noteDetailsView = noteDetailsView;
            _notePreviewView = notePreviewView;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("Custom Notes");
                showBackButton = true;
            }
            ProvideInitialViewControllers(_noteListView, _noteDetailsView, _notePreviewView);
            _noteListView.customNoteChanged += _noteDetailsView.OnNoteWasChanged;
            _noteListView.customNoteChanged += _notePreviewView.OnNoteWasChanged;
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _noteListView.customNoteChanged -= _noteDetailsView.OnNoteWasChanged;
            _noteListView.customNoteChanged -= _notePreviewView.OnNoteWasChanged;
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // Dismiss ourselves
            _mainFlow.DismissFlowCoordinator(this, null);
        }
    }
}