using CustomNotes.Data;
using CustomNotes.Settings.Utilities;
using SiraUtil.Objects;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    internal class CustomBombController : CustomBombControllerBase
    {
        [Inject]
        internal void Init(PluginConfig pluginConfig, NoteAssetLoader noteAssetLoader, [Inject(Id = "cn.bomb")] SiraPrefabContainer.Pool bombContainerPool)
        {
            _pluginConfig = pluginConfig;
            _noteSize = pluginConfig.NoteSize;

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
    }

    internal abstract class CustomBombControllerBase : MonoBehaviour
    {
        protected PluginConfig _pluginConfig;

        protected CustomNote _customNote;
        protected NoteMovement _noteMovement;
        protected NoteController _bombNoteController;

        protected float _noteSize = 1f;

        protected Transform bombMesh;

        protected GameObject activeNote;
        protected SiraPrefabContainer container;
        protected SiraPrefabContainer.Pool bombPool;

        protected virtual void DidFinish()
        {
            container.transform.SetParent(null);
            bombPool.Despawn(container);
        }

        protected virtual void Controller_Init(NoteController noteController)
        {
            SpawnThenParent(bombPool);
        }

        protected virtual void ParentNote(GameObject fakeMesh)
        {
            fakeMesh.SetActive(true);
            container.transform.SetParent(bombMesh);
            fakeMesh.transform.localPosition = container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localRotation = Quaternion.identity;
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f) * _noteSize;
            container.transform.localScale = Vector3.one;
        }

        protected virtual void SpawnThenParent(SiraPrefabContainer.Pool bombModelPool)
        {
            container = bombModelPool.Spawn();
            activeNote = container.Prefab;
            bombPool = bombModelPool;
            ParentNote(activeNote);
        }

        protected virtual void OnDestroy()
        {
            if (_bombNoteController != null)
            {
                _bombNoteController.didInitEvent-= Controller_Init;
            }
        }
    }
}
