using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    /// <summary>
    /// Multiplayer Game Note Controllers don't emit the NoteCut and NoteMissed Events, this is a replacement
    /// </summary>
    public class CustomMultiplayerNoteEventManager : IDisposable
    {

        private class ActiveItemTracker<T> : IDisposable where T : MonoBehaviour
        {

            public HashSet<T> activeItems { get; private set; } = new HashSet<T>();

            public ActiveItemTracker() {}

            public void OnSpawn(T item)
            {
                this.activeItems.Add(item);
            }
            public void OnDespawn(T item)
            {
                this.activeItems.Remove(item);
            }

            public bool Contains(T item)
            {
                return this.activeItems.Contains(item);
            }

            public void Dispose()
            {
                this.activeItems = null;
            }

        }

        public event Action<MultiplayerConnectedPlayerGameNoteController, NoteCutInfo> onGameNoteCutEvent;
        public event Action<MultiplayerConnectedPlayerGameNoteController> onGameNoteMissEvent;

        private static readonly IPA.Utilities.FieldAccessor<UnitVector3Serializable, UnityEngine.Vector3>.Accessor _vUnitAccessor = IPA.Utilities.FieldAccessor<UnitVector3Serializable, UnityEngine.Vector3>.GetAccessor("_v");
        private static readonly IPA.Utilities.FieldAccessor<Vector3Serializable, UnityEngine.Vector3>.Accessor _vAccessor = IPA.Utilities.FieldAccessor<Vector3Serializable, UnityEngine.Vector3>.GetAccessor("_v");

        private IConnectedPlayerNoteEventManager _connectedPlayerNoteEventManager;

        private ActiveItemTracker<MultiplayerConnectedPlayerGameNoteController> _activeMultiGameNoteControllers = new ActiveItemTracker<MultiplayerConnectedPlayerGameNoteController>();

        public CustomMultiplayerNoteEventManager(IConnectedPlayerNoteEventManager connectedPlayerNoteEventManager)
        {
            _connectedPlayerNoteEventManager = connectedPlayerNoteEventManager;
            connectedPlayerNoteEventManager.connectedPlayerNoteWasCutEvent += HandleNoteWasCutEvent;
            connectedPlayerNoteEventManager.connectedPlayerNoteWasMissedEvent += HandleNoteWasMissedEvent;
        }

        public void Dispose()
        {
            _connectedPlayerNoteEventManager.connectedPlayerNoteWasCutEvent -= HandleNoteWasCutEvent;
            _connectedPlayerNoteEventManager.connectedPlayerNoteWasMissedEvent -= HandleNoteWasMissedEvent;
        }

        public void GameNoteSpawnedCallback(MultiplayerConnectedPlayerGameNoteController noteController)
        {
            if(!_activeMultiGameNoteControllers.Contains(noteController))
            {
                _activeMultiGameNoteControllers.OnSpawn(noteController);
            }
        }

        //private bool _isDownStackingGameNotes = false;
        //private Stack _despawnGNCStack = new Stack(5);

        public void GameNoteDespawnedCallback(MultiplayerConnectedPlayerGameNoteController noteController)
        {
            //_despawnGNCStack.Push(noteController);
        }

        /*public void ManageGameNoteDespawnStack()
        {
            _isDownStackingGameNotes = true;
            while (_despawnGNCStack.Count > 0)
            {
                _activeMultiGameNoteControllers.OnDespawn(_despawnGNCStack.Pop() as MultiplayerConnectedPlayerGameNoteController);
            }
            _isDownStackingGameNotes = false;
        }*/

        private void HandleNoteWasCutEvent(NoteCutInfoNetSerializable ncins)
        {
            foreach (MultiplayerConnectedPlayerGameNoteController noteController in _activeMultiGameNoteControllers.activeItems)
            {
                if (CompareNotes(noteController, ncins))
                {
                    this.onGameNoteCutEvent?.Invoke(noteController, ToNCI(ncins));
                    break;
                }
            }
            //if (!_isDownStackingGameNotes) ManageGameNoteDespawnStack();
        }

        private void HandleNoteWasMissedEvent(NoteMissInfoNetSerializable nmins) 
        {
            foreach (MultiplayerConnectedPlayerGameNoteController noteController in _activeMultiGameNoteControllers.activeItems)
            {
                if (CompareNotes(noteController, nmins))
                {
                    this.onGameNoteMissEvent?.Invoke(noteController);
                    break;
                }
            }
            //if (!_isDownStackingGameNotes) ManageGameNoteDespawnStack();
        }

        public static bool CompareNotes(NoteController nc, NoteCutInfoNetSerializable nci)
        {
            NoteData noteData = nc.noteData;
            return Mathf.Approximately(noteData.time, nci.noteTime) && noteData.lineIndex == nci.noteLineIndex && noteData.noteLineLayer == nci.noteLineLayer;
        }

        public static bool CompareNotes(NoteController nc, NoteMissInfoNetSerializable nmi)
        {
            NoteData noteData = nc.noteData;
            return Mathf.Approximately(noteData.time, nmi.noteTime) && noteData.lineIndex == nmi.noteLineIndex && noteData.noteLineLayer == nmi.noteLineLayer;
        }

        public static NoteCutInfo ToNCI(NoteCutInfoNetSerializable i)
        {
            return new NoteCutInfo(
                i.cutWasOk,
                i.cutWasOk,
                i.cutWasOk,
                !i.cutWasOk,
                i.saberSpeed,
                _vUnitAccessor(ref i.saberDir),
                ColorTypeToSaberType(i.colorType),
                0,
                0,
                _vAccessor(ref i.cutPoint),
                _vUnitAccessor(ref i.cutNormal),
                0,
                0,
                null
                );
        }

        public static SaberType ColorTypeToSaberType(ColorType ct)
        {
            return ct == ColorType.ColorA ? SaberType.SaberA : SaberType.SaberB;
        }

    }
}
