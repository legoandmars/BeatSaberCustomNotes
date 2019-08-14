using UnityEngine;

namespace CustomNotes
{
    [RequireComponent(typeof(EventManager))]
    public class EventFilterBehaviour : MonoBehaviour
    {
        protected EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = GetComponent<EventManager>();
                }
                return _eventManager;
            }
        }

        private EventManager _eventManager;
    }
}
