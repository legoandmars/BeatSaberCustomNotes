using CustomUI.BeatSaber;
using IPA.Utilities;
using VRUI;
using System;
using System.Diagnostics;
using UnityEngine;
namespace CustomNotes.UI
{
    class NoteListFlowCoordinator : FlowCoordinator
    {
        CustomNotesUI ui;

        public NoteListViewController noteListViewController;
        public MainFlowCoordinator mainFlowCoordinator;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            Console.WriteLine("MOINEOIG");
            if (firstActivation)
            {
                title = "Custom Notes";

                ui = CustomNotesUI.Instance;
                noteListViewController = BeatSaberUI.CreateViewController<NoteListViewController>();
                noteListViewController.backButtonPressed += Dismiss;
            }

            /*if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(noteListViewController, null, null);
            }*/
        }

        void Dismiss() => ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}
