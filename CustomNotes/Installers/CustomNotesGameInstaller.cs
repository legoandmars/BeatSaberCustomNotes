using CustomNotes.Managers;
using CustomNotes.Providers;
using Zenject;

namespace CustomNotes.Installers
{
    internal class CustomNotesGameInstaller : Installer
    {
        private readonly NoteAssetLoader _noteAssetLoader;

        public CustomNotesGameInstaller(NoteAssetLoader noteAssetLoader)
        {
            _noteAssetLoader = noteAssetLoader;
        }

        public override void InstallBindings()
        {
            if (_noteAssetLoader.SelectedNote != 0)
            {
                Container.Bind<IInitializable>().To<CustomNoteManager>().AsSingle();
                Container.BindInterfacesAndSelfTo<ConnectedPlayerNotePoolProvider>().AsSingle();
            }
        }
    }
}