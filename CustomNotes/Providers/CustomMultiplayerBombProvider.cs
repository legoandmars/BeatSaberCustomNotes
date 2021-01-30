using CustomNotes.Managers;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Interfaces;
using SiraUtil.Objects;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Zenject;

namespace CustomNotes.Providers
{
    internal class CustomMultiplayerBombProvider : IModelProvider
    {
        public Type Type => typeof(CustomMultiplayerBombDecorator);
        public int Priority { get; set; } = 300;

        internal class CustomMultiplayerBombDecorator : IPrefabProvider<MultiplayerConnectedPlayerBombNoteController>
        {
            public bool Chain => true;
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(PluginConfig pluginConfig, GameplayCoreSceneSetupData sceneSetupData)
            {
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows) && pluginConfig.OtherPlayerMultiplayerNotes;
            }

            public MultiplayerConnectedPlayerBombNoteController Modify(MultiplayerConnectedPlayerBombNoteController original)
            {
                if (!CanSetup) return original;
                original.gameObject.AddComponent<CustomMultiplayerBombController>();
                return original;
            }
            
        }

    }

}