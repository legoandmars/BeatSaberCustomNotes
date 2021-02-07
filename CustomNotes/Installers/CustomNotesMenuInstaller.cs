using CustomNotes.Managers;
using CustomNotes.Settings;
using CustomNotes.Settings.UI;
using SiraUtil;
using Zenject;

namespace CustomNotes.Installers
{
    internal class CustomNotesMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<NotePreviewViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NoteDetailsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NoteListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NotesFlowCoordinator>().FromNewComponentOnNewGameObject($"{nameof(NotesFlowCoordinator)}HostGameObject").AsSingle();

            Container.BindInterfacesAndSelfTo<NoteQuickAccessController>().AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        }
    }
}