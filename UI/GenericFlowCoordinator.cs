using System;
using VRUI;
using CustomUI.BeatSaber;
using IPA.Utilities;

namespace CustomNotes.UI
{
    class GenericFlowCoordinator<TCONT, TRIGHT> : FlowCoordinator where TCONT : VRUIViewController where TRIGHT : VRUIViewController
    {
        private TCONT _contentViewController;
        public TRIGHT _rightViewController;
        public Func<TCONT, string> OnContentCreated;

        CustomNotesUI ui;

        public MainFlowCoordinator mainFlowCoordinator;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            Console.WriteLine("CREATING");
            if (firstActivation)
            {
                ui = CustomNotesUI.Instance;
                _contentViewController = BeatSaberUI.CreateViewController<TCONT>();
                _rightViewController = BeatSaberUI.CreateViewController<TRIGHT>();
                title = OnContentCreated(_contentViewController);
            }
            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(_contentViewController, null, _rightViewController);
            }
        }

        void Dismiss()
        {
            ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });
        }

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}
