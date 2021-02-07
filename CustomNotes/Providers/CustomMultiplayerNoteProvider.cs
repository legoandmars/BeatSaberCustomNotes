using CustomNotes.Managers;
using CustomNotes.Settings.Utilities;
using SiraUtil.Interfaces;
using System;
using Zenject;

namespace CustomNotes.Providers
{
    internal class CustomMultiplayerNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomMultiplayerNoteDecorator);
        public int Priority { get; set; } = 300;

        internal class CustomMultiplayerNoteDecorator : IPrefabProvider<MultiplayerConnectedPlayerGameNoteController>
        {
            public bool Chain => true;
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(DiContainer Container, GameplayCoreSceneSetupData sceneSetupData, PluginConfig pluginConfig)
            {
                //bool isMultiplayer = Container.HasBinding<MultiplayerLevelSceneSetupData>();
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows) && pluginConfig.OtherPlayerMultiplayerNotes;

                Container.BindInterfacesAndSelfTo<CustomMultiplayerNoteEventManager>().AsSingle();
            }

            public MultiplayerConnectedPlayerGameNoteController Modify(MultiplayerConnectedPlayerGameNoteController original)
            {
                if (!CanSetup) return original;
                original.gameObject.AddComponent<CustomMultiplayerNoteController>();
                return original;
            }
        }

    }

}