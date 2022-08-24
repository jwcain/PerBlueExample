using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bestagon.Events
{
    /// <summary>
    /// Command Based Observer Pattern tied to Unity Scene lifespan
    /// </summary>
    public class Notification : Behaviours.ProtectedSceneSingleton<Notification>
    {

        //hasA relationship to allow for this class to derive from a MonoBehaviour to tie it to unity
        private Internal.EventSystem<string> eventSystem = new Internal.EventSystem<string>();

        /// <summary>
        /// Resets the internal data of the event system on the Unity MonoBehaviour's destruction
        /// </summary>
        protected override void Destroy()
        {
            eventSystem.Clear();
        }

        /// <summary>
        /// Sends a notification to the system, invoking registered events
        /// </summary>
        /// <param name="notification"></param>
        /// <returns>If true if there was a listener (an action was invoked)</returns>
        public static object[] Send(string notification) => Instance.eventSystem.Handle(notification);

        /// <summary>
        /// Registers an action to occur when a notification is recieved
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="action"></param>
        public static void Register(string notification, System.Func<object> action) => Instance.eventSystem.Register(notification, action);

        /// <summary>
        /// Deregisters an action to occur when a notification is recieved
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="action"></param>
        public static void Deregister(string notification, System.Func<object> action) => Instance.eventSystem.Deregister(notification, action);
    }
}