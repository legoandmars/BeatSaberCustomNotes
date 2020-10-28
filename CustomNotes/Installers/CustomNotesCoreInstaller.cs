using Zenject;
using CustomNotes.Settings.Utilities;

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
        }
    }
}
