using Zenject;
using CustomNotes.Managers;

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
                Container.BindInterfacesAndSelfTo<GameCameraManager>().AsSingle();
                Container.Bind<IInitializable>().To<CustomNoteManager>().AsSingle();
            }
        }
    }
}