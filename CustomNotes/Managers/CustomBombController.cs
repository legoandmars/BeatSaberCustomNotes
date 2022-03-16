﻿using Zenject;
using UnityEngine;
using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using CustomNotes.Utilities;
using SiraUtil.Objects;

namespace CustomNotes.Managers
{
    internal class CustomBombController : MonoBehaviour, INoteControllerDidInitEvent, INoteControllerNoteWasCutEvent, INoteControllerNoteWasMissedEvent, INoteControllerNoteDidDissolveEvent
    {
        private PluginConfig _pluginConfig;

        private CustomNote _customNote;
        private NoteMovement _noteMovement;
        private BombNoteController _bombNoteController;

        protected Transform bombMesh;
        protected GameObject fakeFirstPersonBombMesh;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool bombPool;

        [Inject]
        internal void Init(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, [InjectOptional(Id = Protocol.BombPool)] SiraPrefabContainer.Pool bombContainerPool)
        {
            _pluginConfig = pluginConfig;

            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedBomb];
            bombPool = bombContainerPool;

            _bombNoteController = GetComponent<BombNoteController>();
            _noteMovement = GetComponent<NoteMovement>();

            if (bombPool != null)
            {
                _bombNoteController.didInitEvent.Add(this);
                _bombNoteController.noteWasCutEvent.Add(this);
                _bombNoteController.noteWasMissedEvent.Add(this);
                _bombNoteController.noteDidDissolveEvent.Add(this);
            }

            bombMesh = gameObject.transform.Find("Mesh");
            MeshRenderer bm = GetComponentInChildren<MeshRenderer>();

            if (_pluginConfig.HMDOnly || LayerUtils.HMDOverride)
            {
                if (bombPool == null)
                {
                    // create fake bombs for Custom Notes without Custom Bombs
                    fakeFirstPersonBombMesh = Instantiate(bombMesh.gameObject);
                    fakeFirstPersonBombMesh.name = "FakeFirstPersonBomb";
                    fakeFirstPersonBombMesh.transform.parent = bombMesh;

                    fakeFirstPersonBombMesh.transform.localScale = Vector3.one;
                    fakeFirstPersonBombMesh.transform.localPosition = Vector3.zero;
                    fakeFirstPersonBombMesh.transform.rotation = Quaternion.identity;
                    fakeFirstPersonBombMesh.layer = (int)LayerUtils.NoteLayer.FirstPerson;
                }

            }
            else if (bombPool != null)
            {
                bm.enabled = false;
            }
        }

        private void DidFinish()
        {
            container.Prefab.SetActive(false);
            container.transform.SetParent(null);
            bombPool.Despawn(container);
        }

        public void HandleNoteControllerDidInit(NoteControllerBase noteController)
        {
            SpawnThenParent(bombPool);
        }

        private void ParentNote(GameObject fakeMesh)
        {
            fakeMesh.SetActive(true);
            container.transform.SetParent(bombMesh);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localRotation = Quaternion.identity;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * Utils.NoteSizeFromConfig(_pluginConfig);
            container.transform.localScale = Vector3.one;
        }

        private void SpawnThenParent(SiraPrefabContainer.Pool bombModelPool)
        {
            container = bombModelPool.Spawn();
            container.Prefab.SetActive(true);
            activeNote = container.Prefab;
            bombPool = bombModelPool;
            if (_pluginConfig.HMDOnly == true || LayerUtils.HMDOverride == true)
            {
                LayerUtils.SetLayer(activeNote, LayerUtils.NoteLayer.FirstPerson);
            }
            else
            {
                LayerUtils.SetLayer(activeNote, LayerUtils.NoteLayer.Note);
            }
            ParentNote(activeNote);
        }

        protected void OnDestroy()
        {
            if (_bombNoteController != null)
            {
                _bombNoteController.didInitEvent.Remove(this);
                _bombNoteController.noteWasCutEvent.Remove(this);
                _bombNoteController.noteWasMissedEvent.Remove(this);
                _bombNoteController.noteDidDissolveEvent.Remove(this);
            }
            Destroy(fakeFirstPersonBombMesh);
        }

        public void HandleNoteControllerNoteDidDissolve(NoteController _)
        {
            DidFinish();
        }

        public void HandleNoteControllerNoteWasCut(NoteController _, in NoteCutInfo __)
        {
            DidFinish();
        }

        public void HandleNoteControllerNoteWasMissed(NoteController _)
        {
            DidFinish();
        }
    }
}
