using System;
using Zenject;
using UnityEngine;
using SiraUtil.Objects;
using SiraUtil.Interfaces;
using CustomNotes.Managers;
using CustomNotes.Utilities;
using System.Collections.Generic;
using CustomNotes.Data;
using System.Security.Cryptography;
using System.Text;
using CustomNotes.Settings.Utilities;

namespace CustomNotes.Providers
{
    internal class CustomMultiplayerBombProvider : IModelProvider
    {
        public Type Type => typeof(CustomMultiplayerBombDecorator);
        public int Priority { get; set; } = 300;

        private class CustomMultiplayerBombDecorator : IPrefabProvider<MultiplayerConnectedPlayerBombNoteController>
        {
            private static MD5 _staticMd5Hasher = MD5.Create();

            public bool Chain => true;
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container, PluginConfig pluginConfig, GameplayCoreSceneSetupData sceneSetupData, [Inject(Id = "sirautil.connectedplayer")]IConnectedPlayer connectedPlayer) // , IConnectedPlayer connectedPlayer
            {
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows) && pluginConfig.OtherPlayerMultiplayerNotes;

                if (_noteAssetLoader.SelectedNote != 0 && CanSetup) {
                    // maybe get the custom notes of other players somehow with mpex & download them from modelsaber if they're not installed

                    System.Random rng; 

                    if (pluginConfig.RandomnessIsConsistentPerPlayer) {
                        
                        var hashed = _staticMd5Hasher.ComputeHash(Encoding.UTF8.GetBytes(connectedPlayer.userId));
                        var ivalue = BitConverter.ToInt32(hashed, 0);
                        rng = new System.Random(ivalue);
                    } else {
                        rng = new System.Random();
                    }
                    
                    var note = pluginConfig.RandomMultiplayerNotes ? _noteAssetLoader.CustomNoteObjects[rng.Next(1, _noteAssetLoader.CustomNoteObjects.Count)] : _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    if(note.NoteBomb == null) {
                        CanSetup = false;
                        return;
                    }
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteBomb);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.multi.bomb").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteBomb));
                }
            }

            public MultiplayerConnectedPlayerBombNoteController Modify(MultiplayerConnectedPlayerBombNoteController original)
            {
                if (!CanSetup) return original;
                original.gameObject.AddComponent<CustomMultiplayerBombController>();
                return original;
            }

            private SiraPrefabContainer NotePrefabContainer(GameObject initialPrefab)
            {
                var prefab = new GameObject("CustomNotes_Multi" + initialPrefab.name).AddComponent<SiraPrefabContainer>();
                prefab.Prefab = initialPrefab;
                return prefab;
            }

        }

    }

}