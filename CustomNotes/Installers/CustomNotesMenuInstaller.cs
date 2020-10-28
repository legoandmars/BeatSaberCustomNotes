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
            Container.BindViewController<NotePreviewViewController>();
            Container.BindViewController<NoteDetailsViewController>();
            Container.BindViewController<NoteListViewController>();
            Container.BindFlowCoordinator<NotesFlowCoordinator>();

            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        }
    }
}