using CustomNotes.Installers;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using SiraUtil.Zenject;
using System.IO;
using IPALogger = IPA.Logging.Logger;

namespace CustomNotes
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomNotes");

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.log = logger;

            PluginConfig pluginConfig = config.Generated<PluginConfig>();
            LayerUtils.pluginConfig = pluginConfig;
            zenjector.Install(Location.App, Container => Container.BindInstance(pluginConfig).AsSingle());
            zenjector.Install<CustomNotesCoreInstaller>(Location.App);
            zenjector.Install<CustomNotesMenuInstaller>(Location.Menu);
            zenjector.Install<CustomNotesGameInstaller>(Location.Player);
        }
    }
}