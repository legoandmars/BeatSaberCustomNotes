using Zenject;
using SiraUtil.Interfaces;
using CustomNotes.Managers;
using CustomNotes.Providers;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;

namespace CustomNotes.Installers
{
    internal class CustomNotesCoreInstaller : Installer<PluginConfig, CustomNotesCoreInstaller>
    {
        private readonly PluginConfig _pluginConfig;

        public CustomNotesCoreInstaller(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
            LayerUtils.pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_pluginConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<NoteAssetLoader>().AsSingle();

            Container.Bind(typeof(IModelProvider), typeof(CustomGameNoteProvider)).To<CustomGameNoteProvider>().AsSingle();
            Container.Bind(typeof(IModelProvider), typeof(CustomBombNoteProvider)).To<CustomBombNoteProvider>().AsSingle();
        }
    }
}