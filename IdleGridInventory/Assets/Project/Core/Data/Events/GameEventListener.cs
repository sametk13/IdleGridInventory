using UnityEngine;

namespace OmniGameTemplate.Core.Data.Events
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent Event;
        public UnityEngine.Events.UnityEvent Response;

        private void OnEnable() => Event?.Register(this);
        private void OnDisable() => Event?.Unregister(this);
        public void OnEventRaised() => Response?.Invoke();
    }
}
