using System;
using System.Collections.Generic;

namespace App.Core.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
            }
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] is Action<T> action)
                    {
                        action(eventData);
                    }
                }
            }
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
