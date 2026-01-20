using System.Collections.Generic;
using UnityEngine;

namespace OmniGameTemplate.Core.Data.Events
{
    [CreateAssetMenu(menuName = "OmniGameTemplate/Events/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new();
        public void Raise() { for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnEventRaised(); }
        public void Register(GameEventListener l) => _listeners.Add(l);
        public void Unregister(GameEventListener l) => _listeners.Remove(l);
    }
}
