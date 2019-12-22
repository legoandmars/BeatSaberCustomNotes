using IPA.Utilities;
using HMUI;
using System;
using BeatSaberMarkupLanguage;
namespace CustomNotes.UI
{
    class CustomNotesFlowCoordinator : FlowCoordinator
    {
        private NotesListView _notesListView;
        public void Awake()
        {
            if (_notesListView == null)
            {
                _notesListView = BeatSaberUI.CreateViewController<NotesListView>();
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
                    ProvideInitialViewControllers(_notesListView);
                }
                if (activationType == ActivationType.AddedToHierarchy)
                {

                }

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // dismiss ourselves
            var mainFlow = BeatSaberMarkupLanguage.BeatSaberUI.MainFlowCoordinator;
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", this, null, false);
        }
    }

}
