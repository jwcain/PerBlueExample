using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bestagon.Behaviours
{
    /// <summary>
    /// A singleton behaviour that exists for the lifecycle of a scene.
    /// An instance is spawned if one is not found in the scene during initial acess
    /// Instance is public
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        /// <summary>
        /// Internal reference to the type
        /// </summary>
        private static T instance;

        /// <summary>
        /// Access the scene instance of a static Behaviour
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        //Make one
                        instance = (new GameObject(typeof(T).Name + " (SingletonBehaviour)")).AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        private void OnDestroy()
        {
            //Destroy first to avoid having self-references spawn a new instance
            Destroy();
            //Clear the internal instance
            instance = null;
        }

        /// <summary>
        /// Returns if the instance already exists
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            return instance != null;
        }

        /// <summary>
        /// Called when the StaticBehaviour is destroyed.
        /// Deletes/Cleans static state
        /// </summary>
        protected abstract void Destroy();
    }
}