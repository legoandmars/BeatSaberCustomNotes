using Zenject;
using SiraUtil;
using CustomNotes.Managers;
using CustomNotes.Settings;
using CustomNotes.Settings.UI;

namespace CustomNotes.Installers
{
    internal class CustomNotesMenuInstaller : Installer
    {
        public override void InstallBindings()
        {


            Container.Bind<NotePreviewViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NoteDetailsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NoteListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<NoteModifierViewController>().AsSingle();
            Container.Bind<NotesFlowCoordinator>().FromNewComponentOnNewGameObject(nameof(NotesFlowCoordinator)).AsSingle();

            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
            Container.BindInterfacesTo<CustomNotesViewManager>().AsSingle();
        }
    }
}