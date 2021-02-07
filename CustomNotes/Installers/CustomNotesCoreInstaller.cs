using CustomNotes.Managers;
using CustomNotes.Providers;
using CustomNotes.Settings.Utilities;
using IPA.Loader;
using SiraUtil.Interfaces;
using Zenject;

namespace CustomNotes.Installers
{
    internal class CustomNotesCoreInstaller : Installer<PluginConfig, CustomNotesCoreInstaller>
    {
        private readonly PluginConfig _pluginConfig;

        public CustomNotesCoreInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_pluginConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<NoteAssetLoader>().AsSingle();

            Container.Bind(typeof(IModelProvider), typeof(CustomGameNoteProvider)).To<CustomGameNoteProvider>().AsSingle();
            Container.Bind(typeof(IModelProvider), typeof(CustomBombNoteProvider)).To<CustomBombNoteProvider>().AsSingle();

            Container.Bind(typeof(IModelProvider), typeof(CustomMultiplayerNoteProvider)).To<CustomMultiplayerNoteProvider>().AsSingle();
            Container.Bind(typeof(IModelProvider), typeof(CustomMultiplayerBombProvider)).To<CustomMultiplayerBombProvider>().AsSingle();

            if (PluginManager.GetPluginFromId("MultiplayerExtensions") != null)
            {
                Container.BindInterfacesAndSelfTo<CustomNotesNetworkPacketManager>().AsSingle();
            }
            else
            {
                Container.BindInterfacesAndSelfTo<DummyCustomNotesNetworkPacketManager>().AsSingle();
            }
        }
    }
}