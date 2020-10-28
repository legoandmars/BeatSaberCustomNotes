using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomNotes.Data;
using CustomNotes.Utilities;
using HMUI;
using System;
using System.Linq;
using UnityEngine;

namespace CustomNotes.Settings.UI
{
    internal class NoteListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteList.bsml";

        private bool isGeneratingPreview = false;
        private GameObject preview;

        // Notes
        private GameObject noteLeft;
        private GameObject noteDotLeft;
        private GameObject noteRight;
        private GameObject noteDotRight;
        private GameObject noteBomb;

        // NoteArrows
        private CustomNote fakeNoteArrows = null;
        private GameObject fakeLeftArrow;
        private GameObject fakeLeftDot;
        private GameObject fakeRightArrow;
        private GameObject fakeRightDot;

        // NotePositions (Local to the previewer)
        private Vector3 leftDotPos = new Vector3(0.0f, 1.5f, 0.0f);
        private Vector3 leftArrowPos = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 rightDotPos = new Vector3(1.5f, 1.5f, 0.0f);
        private Vector3 rightArrowPos = new Vector3(1.5f, 0.0f, 0.0f);
        private Vector3 bombPos = new Vector3(3.0f, 0.75f, 0.0f);

        public Action<CustomNote> customNoteChanged;

        [UIComponent("noteList")]
        public CustomListTableData customListTableData = null;

        [UIAction("noteSelect")]
        public void Select(TableView _, int row)
        {
            NoteAssetLoader.SelectedNote = row;
            Configuration.CurrentlySelectedNote = NoteAssetLoader.CustomNoteObjects[row].FileName;

            GenerateNotePreview(row);
        }

        [UIAction("reloadNotes")]
        public void ReloadNotes()
        {
            NoteAssetLoader.Reload();
            SetupList();
            Select(customListTableData.tableView, NoteAssetLoader.SelectedNote);
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();

            foreach (CustomNote note in NoteAssetLoader.CustomNoteObjects)
            {
                Sprite sprite = Sprite.Create(note.Descriptor.Icon, new Rect(0, 0, note.Descriptor.Icon.width, note.Descriptor.Icon.height), new Vector2(0.5f, 0.5f));
                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(note.Descriptor.NoteName, note.Descriptor.AuthorName, sprite);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            int selectedNote = NoteAssetLoader.SelectedNote;

            customListTableData.tableView.ScrollToCellWithIdx(selectedNote, TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedNote);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            if (fakeNoteArrows == null)
            {
                byte[] resource = Utils.LoadFromResource("CustomNotes.Resources.Notes.cn_arrowplaceholder.bloq");
                fakeNoteArrows = new CustomNote(resource, "cn_arrowplaceholder.bloq");
            }

            if (!preview)
            {
                preview = new GameObject();
                preview.transform.Rotate(0.0f, 60.0f, 0.0f);
                preview.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }

            Select(customListTableData.tableView, NoteAssetLoader.SelectedNote);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
            ClearPreview();
        }

        private void GenerateNotePreview(int selectedNote)
        {
            if (!isGeneratingPreview)
            {
                try
                {
                    isGeneratingPreview = true;
                    ClearNotes();

                    CustomNote currentNote = NoteAssetLoader.CustomNoteObjects[selectedNote];
                    if (currentNote != null)
                    {
                        customNoteChanged?.Invoke(currentNote);
                        InitializePreviewNotes(currentNote, preview.transform);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log.Error(ex);
                }
                finally
                {
                    isGeneratingPreview = false;
                }
            }
        }

        private void InitializePreviewNotes(CustomNote customNote, Transform transform)
        {
            // Position previewer based on the CustomNote having a NoteBomb
            preview.transform.position = customNote.NoteBomb ? new Vector3(2.1f, 0.9f, 1.60f) : new Vector3(2.25f, 0.9f, 1.45f);

            noteLeft = CreatePreviewNote(customNote.NoteLeft, transform, leftArrowPos);
            noteDotLeft = CreatePreviewNote(customNote.NoteDotLeft, transform, leftDotPos);
            noteRight = CreatePreviewNote(customNote.NoteRight, transform, rightArrowPos);
            noteDotRight = CreatePreviewNote(customNote.NoteDotRight, transform, rightDotPos);
            noteBomb = CreatePreviewNote(customNote.NoteBomb, transform, bombPos);

            // Fake the Note Dots if no CustomNote Dot existed in the CustomNote
            if (noteLeft && !noteDotLeft)
            {
                noteDotLeft = CreatePreviewNote(customNote.NoteLeft, transform, leftDotPos);
            }
            if (noteRight && !noteDotRight)
            {
                noteDotRight = CreatePreviewNote(customNote.NoteRight, transform, rightDotPos);
            }

            // Add arrows to arrowless notes
            if (!customNote.Descriptor.DisableBaseNoteArrows && fakeNoteArrows != null)
            {
                if (noteLeft && noteRight)
                {
                    AddNoteArrows(fakeNoteArrows, transform);
                }
            }

            if (customNote.Descriptor.UsesNoteColor)
            {
                ColorManager colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().First();
                if (colorManager)
                {
                    float colorStrength = customNote.Descriptor.NoteColorStrength;
                    Color noteAColor = colorManager.ColorForType(ColorType.ColorA);
                    Color noteBColor = colorManager.ColorForType(ColorType.ColorB);

                    Utils.ColorizeCustomNote(noteAColor, colorStrength, noteLeft);
                    Utils.ColorizeCustomNote(noteBColor, colorStrength, noteRight);
                    Utils.ColorizeCustomNote(noteBColor, colorStrength, noteDotRight);
                    Utils.ColorizeCustomNote(noteAColor, colorStrength, noteDotLeft);
                }
                else
                {
                    Logger.log.Warn("Failed to locate a suitable ColorManager for the CustomNote preview");
                }
            }
        }

        private GameObject CreatePreviewNote(GameObject note, Transform transform, Vector3 localPosition)
        {
            GameObject noteObject = InstantiateGameObject(note, transform);
            PositionPreviewNote(localPosition, noteObject);
            return noteObject;
        }

        private void AddNoteArrows(CustomNote customNote, Transform transform)
        {
            fakeLeftArrow = CreatePreviewNote(customNote.NoteLeft, transform, leftArrowPos);
            fakeLeftDot = CreatePreviewNote(customNote.NoteDotLeft, transform, leftDotPos);
            fakeRightArrow = CreatePreviewNote(customNote.NoteRight, transform, rightArrowPos);
            fakeRightDot = CreatePreviewNote(customNote.NoteDotRight, transform, rightDotPos);
        }

        private GameObject InstantiateGameObject(GameObject gameObject, Transform transform = null)
        {
            if (gameObject)
            {
                return transform ? Instantiate(gameObject, transform) : Instantiate(gameObject);
            }

            return null;
        }

        private void PositionPreviewNote(Vector3 vector, GameObject noteObject)
        {
            if (noteObject && vector != null)
            {
                noteObject.transform.localPosition = vector;
            }
        }

        private void ClearPreview()
        {
            ClearNotes();
            DestroyGameObject(ref preview);
        }

        private void ClearNotes()
        {
            DestroyGameObject(ref noteLeft);
            DestroyGameObject(ref noteDotLeft);
            DestroyGameObject(ref noteRight);
            DestroyGameObject(ref noteDotRight);
            DestroyGameObject(ref noteBomb);

            DestroyGameObject(ref fakeLeftArrow);
            DestroyGameObject(ref fakeLeftDot);
            DestroyGameObject(ref fakeRightArrow);
            DestroyGameObject(ref fakeRightDot);
        }

        private void DestroyGameObject(ref GameObject gameObject)
        {
            if (gameObject)
            {
                Destroy(gameObject);
                gameObject = null;
            }
        }
    }
}
