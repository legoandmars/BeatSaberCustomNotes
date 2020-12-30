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
    internal class CustomMultiplayerConnectedPlayerGameNoteProvider : IModelProvider
    {
        public Type Type => typeof(CustomMultiplayerConnectedPlayerGameNoteDecorator);
        public int Priority { get; set; } = 300;

        private class CustomMultiplayerConnectedPlayerGameNoteDecorator : IPrefabProvider<MultiplayerConnectedPlayerGameNoteController>
        {

            private static readonly IPA.Utilities.FieldAccessor<MultiplayerSessionManager, List<IConnectedPlayer>>.Accessor _connectedPlayersAccessor = IPA.Utilities.FieldAccessor<MultiplayerSessionManager, List<IConnectedPlayer>>.GetAccessor("_connectedPlayers");

            private static MD5 _staticMd5Hasher = MD5.Create();

            public bool Chain => true;
            public bool CanSetup { get; private set; }

            [Inject]
            public void Construct(NoteAssetLoader _noteAssetLoader, DiContainer Container, PluginConfig pluginConfig, GameplayCoreSceneSetupData sceneSetupData, [Inject(Id = "sirautil.connectedplayer")] IConnectedPlayer connectedPlayer) // , IConnectedPlayer connectedPlayer
            {
                bool isMultiplayer = Container.HasBinding<MultiplayerLevelSceneSetupData>();
                CanSetup = !(sceneSetupData.gameplayModifiers.ghostNotes || sceneSetupData.gameplayModifiers.disappearingArrows);

                if (_noteAssetLoader.SelectedNote != 0) {
                    // maybe get the custom notes of other players somehow with mpex & download them from modelsaber if they're not installed

                    System.Random rng;

                    if (pluginConfig.RandomnessIsConsistentPerPlayer) {
                        // Set the Random seed based on player ID -> same random number every time for the same player, hashing is probably unnecessary but eh ¯\_(ツ)_/¯
                        var hashed = _staticMd5Hasher.ComputeHash(Encoding.UTF8.GetBytes(connectedPlayer?.userId));
                        var ivalue = BitConverter.ToInt32(hashed, 0);
                        rng = new System.Random(ivalue);
                    } else {
                        rng = new System.Random();
                    }

                    var note = pluginConfig.RandomMultiplayerNotes ? _noteAssetLoader.CustomNoteObjects[rng.Next(1, _noteAssetLoader.CustomNoteObjects.Count)] : _noteAssetLoader.CustomNoteObjects[_noteAssetLoader.SelectedNote];
                    MaterialSwapper.GetMaterials();
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteLeft);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteRight);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotLeft);
                    MaterialSwapper.ReplaceMaterialsForGameObject(note.NoteDotRight);
                    Utils.AddMaterialPropertyBlockController(note.NoteLeft);
                    Utils.AddMaterialPropertyBlockController(note.NoteRight);
                    Utils.AddMaterialPropertyBlockController(note.NoteDotLeft);
                    Utils.AddMaterialPropertyBlockController(note.NoteDotRight);

                    Container.Bind<CustomNote>().WithId("cn.multi.note").FromInstance(note);
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.multi.left.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteLeft));
                    Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.multi.right.arrow").WithInitialSize(25).FromComponentInNewPrefab(NotePrefabContainer(note.NoteRight));
                    if (note.NoteDotLeft != null) {
                        Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.multi.left.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotLeft));
                    }
                    if (note.NoteDotRight != null) {
                        Container.BindMemoryPool<SiraPrefabContainer, SiraPrefabContainer.Pool>().WithId("cn.multi.right.dot").WithInitialSize(10).FromComponentInNewPrefab(NotePrefabContainer(note.NoteDotRight));
                    }
                    Container.Bind<CustomMultiplayerNoteEventManager>().AsSingle();
                    Logger.log.Debug($"Bindings for player: {connectedPlayer.userName} - ID: {connectedPlayer.userId} installed!");
                }
            }

            public MultiplayerConnectedPlayerGameNoteController Modify(MultiplayerConnectedPlayerGameNoteController original)
            {
                if (!CanSetup) return original;
                original.gameObject.AddComponent<CustomMultiplayerConnectedPlayerGameNoteController>();
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