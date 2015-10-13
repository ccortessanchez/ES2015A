﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Utils
{
    public abstract class Actor<T> : UnityEngine.MonoBehaviour, IObserver<T>, IActor<T> where T : struct, IConvertible
    {
        private Dictionary<T, List<Action<Object>>> callbacks = new Dictionary<T, List<Action<Object>>>();
        private List<AutoUnregister<T>> autoUnregisters = new List<AutoUnregister<T>>();

        public Actor()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (T action in Enum.GetValues(typeof(T)))
            {
                callbacks.Add(action, new List<Action<Object>>());
            }
        }

        public virtual void Start() { }
        public virtual void Update() { }

        public virtual void OnDestroy()
        {
            foreach (AutoUnregister<T> auto in autoUnregisters.ToList())
            {
                auto.unregisterAll<T>();
            }

            // This should always be true, as AutoUnregister.unregisterAll
            // automatically unregisters itself from the actor
            UnityEngine.Debug.Assert(autoUnregisters.Count == 0);
        }

        public void register(AutoUnregister<T> auto)
        {
            autoUnregisters.Add(auto);
        }

        public RegisterResult<T> register(T action, Action<Object> func)
        {
            callbacks[action].Add(func);
            return new RegisterResult<T>(this, action, func);
        }

        public void unregister(AutoUnregister<T> auto)
        {
            autoUnregisters.Remove(auto);
        }

        public void unregister(T action, Action<Object> func, bool skipAutoUnregister = false)
        {
            callbacks[action].Remove(func);

            if (!skipAutoUnregister)
            {
                foreach (AutoUnregister<T> auto in autoUnregisters.ToList())
                {
                    auto.unregister(this, action, func);
                }
            }
        }

        protected void fire(T action)
        {
            foreach (Action<Object> func in callbacks[action].ToList())
            {
                func.Invoke(gameObject);
            }
        }

        protected void fire(T action, Object obj)
        {
            foreach (Action<Object> func in callbacks[action].ToList())
            {
                func.Invoke(obj);
            }
        }
    }
}
