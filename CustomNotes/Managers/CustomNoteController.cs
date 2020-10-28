using Zenject;
using UnityEngine;
using CustomNotes.Data;
using CustomNotes.Overrides;
using CustomNotes.Utilities;

namespace CustomNotes.Managers
{
    public class CustomNoteController : MonoBehaviour
    {
        protected Transform noteCube;
        private CustomNote _customNote;
        private GameNoteController _gameNoteController;
        private CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;

        protected GameObject noteA;
        protected GameObject noteB;
        protected GameObject noteADot;
        protected GameObject noteBDot;

        [Inject]
        internal void Init(NoteAssetLoader noteAssetLoader)
        {
            _customNote = noteAssetLoader.CustomNoteObjects[noteAssetLoader.SelectedNote];

            _gameNoteController = GetComponent<GameNoteController>();
            _customNoteColorNoteVisuals = gameObject.AddComponent<CustomNoteColorNoteVisuals>();

            _gameNoteController.didInitEvent += Controller_DidInit;
            _customNoteColorNoteVisuals.didInitEvent += Visuals_DidInit;

            noteCube = _gameNoteController.gameObject.transform.Find("NoteCube");

            MeshRenderer noteMesh = GetComponentInChildren<MeshRenderer>();
            noteMesh.forceRenderingOff = true;
        }

        private void Controller_DidInit(NoteController noteController)
        {
            switch (noteController.noteData.colorType)
            {
                case ColorType.ColorA:
                    if (noteController.noteData.cutDirection == NoteCutDirection.Any) InstantiateThenParent(_customNote.NoteDotLeft, ref noteADot);
                    else InstantiateThenParent(_customNote.NoteLeft, ref noteA);
                    break;
                case ColorType.ColorB:
                    if (noteController.noteData.cutDirection == NoteCutDirection.Any) InstantiateThenParent(_customNote.NoteDotRight, ref noteBDot);
                    else InstantiateThenParent(_customNote.NoteRight, ref noteB);
                    break;
                default:
                    break;
            }
        }

        private void ParentNote(GameObject fakeMesh)
        {
            fakeMesh.transform.SetParent(noteCube);
            fakeMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            fakeMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            fakeMesh.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
            fakeMesh.transform.localRotation = fakeMesh.transform.localRotation * transform.localRotation;
        }

        private void InstantiateThenParent(GameObject note, ref GameObject to)
        {
            if (to == null)
            {
                to = Instantiate(note);
                ParentNote(to);
            }
        }

        protected void SetActiveThenColor(GameObject note, Color color)
        {
            note.SetActive(true);
            Utils.ColorizeCustomNote(color, _customNote.Descriptor.NoteColorStrength, note);
        }

        private void Visuals_DidInit(ColorNoteVisuals visuals, NoteController noteController)
        {
            noteA?.SetActive(false);
            noteB?.SetActive(false);
            noteADot?.SetActive(false);
            noteBDot?.SetActive(false);

            // Check and activate/deactivate custom note objects attached to a pooled note object
            var noteData = noteController.noteData;
            if (noteData.colorType == ColorType.ColorA)
            {
                if (noteData.cutDirection == NoteCutDirection.Any) SetActiveThenColor(noteADot, visuals.noteColor);
                else SetActiveThenColor(noteA, visuals.noteColor);
            }
            else if (noteData.colorType == ColorType.ColorB)
            {
                if (noteData.cutDirection == NoteCutDirection.Any) SetActiveThenColor(noteBDot, visuals.noteColor);
                else SetActiveThenColor(noteB, visuals.noteColor);
            }

            // Hide certain parts of the default note which is not required
            if (_customNote.Descriptor.DisableBaseNoteArrows)
            {
                _customNoteColorNoteVisuals.TurnOffVisuals();
            }
        }

        protected void OnDestroy()
        {
            if (_gameNoteController != null)
            {
                _gameNoteController.didInitEvent -= Controller_DidInit;
            }
            if (_customNoteColorNoteVisuals != null)
            {
                _customNoteColorNoteVisuals.didInitEvent -= Visuals_DidInit;
            }
        }
    }
}