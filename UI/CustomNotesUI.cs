using System;
using System.Linq;
using CustomUI.MenuButton;
using TMPro;
using LogLevel = IPA.Logging.Logger.Level;
using UnityEngine;
using CustomUI.Utilities;

namespace CustomNotes.UI
{
    class CustomNotesUI : MonoBehaviour
    {
        public static CustomNotesUI Instance;
        private MainFlowCoordinator mainFlowCoordinator;
        public NoteListFlowCoordinator noteListFlowCoordinator;

        public class NoteListFlowCoordinator : GenericFlowCoordinator<NoteListViewController, NotePreviewController> { };

        internal static void OnLoad()
        {
            if (Instance != null)
            {
                return;
            }

            new GameObject("CustomNotesUI").AddComponent<CustomNotesUI>();
        }

        private void Awake()
        {
            Instance = this;
            /*try
            {
                mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }*/
            MenuButtonUI.AddButton("Custom Notes", "Select your current custom note", CreateCustomNotesButton);
            //CreateCustomNotesButton();
        }

        private void CreateCustomNotesButton()
        {
            Console.WriteLine("Starting Custom Notes UI");
            if (noteListFlowCoordinator == null)
            {
                noteListFlowCoordinator = new GameObject("NoteListFlowCoordinator").AddComponent<NoteListFlowCoordinator>();
            }
            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlowCoordinator.InvokeMethod("PresentFlowCoordinator", noteListFlowCoordinator, null, false, false);
            /*
            MenuButtonUI.AddButton("Custom Notes", delegate ()
            {
                Console.WriteLine("CREATING4");
                if (noteListFlowCoordinator == null)
                {
                    noteListFlowCoordinator = new GameObject("NoteListFlowCoordinator").AddComponent<NoteListFlowCoordinator>();
                    noteListFlowCoordinator.mainFlowCoordinator = mainFlowCoordinator;
                    Console.WriteLine("CREATING3");
                    noteListFlowCoordinator.OnContentCreated = (content) =>
                    {
                        Console.WriteLine("CREATING2");
                        content.backButtonPressed = () =>
                        {
                            mainFlowCoordinator.InvokePrivateMethod("DismissFlowCoordinator", new object[] { noteListFlowCoordinator, null, false });
                        };
                        return "Custom Notes";
                    };
                    //_mainFlowCoordinator
                }
                ReflectionUtil.InvokePrivateMethod(mainFlowCoordinator, "PresentFlowCoordinator", new object[] { noteListFlowCoordinator, null, false, false });
            });*/
        }
    }
}
