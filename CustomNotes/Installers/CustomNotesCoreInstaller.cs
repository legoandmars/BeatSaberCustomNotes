using CustomNotes.Managers;
using Zenject;

namespace CustomNotes.Installers
{
    internal class CustomNotesCoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NoteAssetLoader>().AsSingle();
        }
    }
}