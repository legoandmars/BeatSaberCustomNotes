using Zenject;
using UnityEngine;
using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using SiraUtil.Objects;

namespace CustomNotes.Managers
{
    internal class CustomBombController : MonoBehaviour
    {
        private PluginConfig _pluginConfig;

        private CustomNote _customNote;
        private NoteMovement _noteMovement;
        private BombNoteController _bombNoteController;

        protected Transform bombMesh;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool bombPool;

        [Inject]
        internal void Init(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, [Inject(Id = "cn.bomb")] SiraPrefabContainer.Pool bombContainerPool)
        {
            _pluginConfig = pluginConfig;

            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];
            _bombNoteController = GetComponent<BombNoteController>();
            _noteMovement = GetComponent<NoteMovement>();
            _bombNoteController.didInitEvent += Controller_Init;
            _noteMovement.noteDidFinishJumpEvent += DidFinish;
            bombMesh = gameObject.transform.Find("Mesh");
            bombPool = bombContainerPool;

            MeshRenderer bm = GetComponentInChildren<MeshRenderer>();
            bm.enabled = false;
        }

        private void DidFinish()
        {
            container.transform.SetParent(null);
            bombPool.Despawn(container);
        }

        private void Controller_Init(NoteController noteController)
        {
            SpawnThenParent(bombPool);
        }

        private void SetNoteLayer(GameObject fakeMesh) {
            if (fakeMesh.layer == bombMesh.gameObject.layer) return;
            fakeMesh.layer = bombMesh.gameObject.layer;
            foreach (Transform t in fakeMesh.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = bombMesh.gameObject.layer;
            }
        }

        private void ParentNote(GameObject fakeMesh)
        {
            fakeMesh.SetActive(true);
            container.transform.SetParent(bombMesh);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localRotation = Quaternion.identity;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * _pluginConfig.NoteSize;
            container.transform.localScale = Vector3.one;
        }

        private void SpawnThenParent(SiraPrefabContainer.Pool bombModelPool)
        {
            container = bombModelPool.Spawn();
            activeNote = container.Prefab;
            bombPool = bombModelPool;
            if (_pluginConfig.NoteMirrorReflection)
                SetNoteLayer(activeNote);
            ParentNote(activeNote);
        }

        protected void OnDestroy()
        {
            if (_bombNoteController != null)
            {
                _bombNoteController.didInitEvent-= Controller_Init;
            }
        }
    }
}
