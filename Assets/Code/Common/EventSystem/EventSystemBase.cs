using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bestagon.Events.Internal
{

    /// <summary>
    /// A Command/Observer System
    /// the Object used here to abstract System.Function<T1, T2, ..., object> to any length
    /// K is key
    /// </summary>
    public abstract class EventSystemBase<K>
    {

        /// <summary>
        /// The full registry of events
        /// </summary>
        public Dictionary<K, List<object>> registry = new Dictionary<K, List<object>>();

        /// <summary>
        /// Stores a temporary set from the registry to avoid modification during invocation
        /// </summary>
        public HashSet<List<object>> CurrentlyInvoking = new HashSet<List<object>>();

        /// <summary>
        /// Removes all entries related to this key
        /// </summary>
        /// <param name="key"></param>
        public void DumpKey(K key)
        {
            registry.Remove(key);
        }

        /// <summary>
        /// Clears internal data
        /// </summary>
        /// <param name="key"></param>
        public void Clear()
        {
            registry = new Dictionary<K, List<object>>();
            CurrentlyInvoking = new HashSet<List<object>>();
        }

        /// <summary>
        /// Performs an operation on the registry with safty checking
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="operation"></param>
        private void SafeRegisteryOperation(K key, object func, System.Action operation)
        {

            //Null actions serve no purpose
            if (func == null)
            {
                Debug.LogError("Null Function was provided.");
                return;
            }

            //Add a new list for this K if we have not seen it before
            if (registry.ContainsKey(key) == false)
                registry.Add(key, new List<object>());

            //We track an invoking list for the case when an Function modifies the key set that called it
            //We do not allow the new addition to get called either
            if (CurrentlyInvoking.Contains(registry[key]))
                registry[key] = new List<object>(registry[key]);

            operation?.Invoke();
        }

        protected void _Register(K key, object func)
        {
            //Preform standard saftey checks and continue operation in a llamda function if the saftey checks pass
            SafeRegisteryOperation(key, func, () =>
            {
                //CAnnot add an func if it already exists within the registry
                if (registry[key].Contains(func))
                {
                    Debug.LogWarning("Attemtped to register a func to the same key multiple times (" + key + ")");
                }
                else
                    registry[key].Add(func);
            });
        }

        protected void _Deregister(K key, object func)
        {
            //Preform standard saftey checks and continue operation in a llamda function if the saftey checks pass
            SafeRegisteryOperation(key, func, () =>
            {
                if (registry[key].Contains(func))
                {
                    registry[key].Remove(func);
                }
                else
                    Debug.LogWarning("Attemtped to unregister a func that was not found.");
            });
        }

        /// <summary>
        /// Handles processessing the set of actions relative to a Key
        /// Handler arg should understand how to process the events stored as methods
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        /// <returns>Bool: if a function was called. object[] </returns>
        protected object[] _Handle(K key, System.Func<object, object> handler)
        {

            if (registry.ContainsKey(key))
            {
                //Get actions list
                List<object> actions = registry[key];

                //None found to perfom
                if (actions.Count == 0)
                    return new object[] { };

                object[] returns = new object[actions.Count];

                //Psuedo-concurrency atomic lock
                CurrentlyInvoking.Add(actions);

                //Invoke all actions
                for (int i = 0; i < actions.Count; i++)
                    returns[i] = handler(actions[i]);

                //Release atomic lock
                CurrentlyInvoking.Remove(actions);

                return returns;

            }
            else
                return new object[] { };
        }

        public override string ToString()
        {
            int keys = 0;
            int registered = 0;
            foreach (var item in registry)
            {
                keys++;
                registered += item.Value.Count;
            }
            return "EventSystem<" + typeof(K).Name + "> Keys:" + keys + "|Func:" + registered;
        }

        public string ToString(bool deep = false)
        {
            if (deep)
            {
                string ret = "EventSystem<" + typeof(K).Name + ">\n";
                foreach (var item in registry)
                {
                    ret += string.Format("K:", item.Key, item.Value.Count) + " F:" + item.Value.Count + "\n";
                }
                return ret;
            }
            else
                return ToString();
        }
    }
}