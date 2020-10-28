using Zenject;
using UnityEngine;
using CustomNotes.Data;

namespace CustomNotes.Managers
{
    internal class CustomBombController : MonoBehaviour
    {
        private CustomNote _customNote;
        private BombNoteController _bombNoteController;

        protected GameObject bomb;
        protected Transform bombMesh;

        [Inject]
        internal void Init(NoteAssetLoader noteAssetLoader)
        {
            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];
            _bombNoteController = GetComponent<BombNoteController>();
            _bombNoteController.didInitEvent += Controller_Init;
            bombMesh = gameObject.transform.Find("Mesh");
        }

        private void Controller_Init(NoteController noteController)
        {
            if (bomb == null)
            {
                if (_customNote.NoteBomb)
                {
                    bomb = Instantiate(_customNote.NoteBomb);
                    bomb.transform.SetParent(bombMesh);
                    bomb.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    bomb.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    bomb.transform.Rotate(new Vector3(0f, 0f, 90f), Space.Self);
                    bomb.transform.localRotation = bomb.transform.localRotation * transform.localRotation;

                    MeshRenderer bm = GetComponentInChildren<MeshRenderer>();
                    bm.enabled = false;
                }
            }
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
