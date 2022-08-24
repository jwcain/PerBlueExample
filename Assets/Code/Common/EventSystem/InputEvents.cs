using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Bestagon.Events
{

    /// <summary>
    /// Type of key event to listen for
    /// </summary>
    public enum KeyEvent
    {
        /// <summary>
        /// Event Triggered on first frame of a press
        /// </summary>
        Down,
        /// <summary>
        /// Event Triggered when the key is released
        /// </summary>
        Up,
        /// <summary>
        /// Event Triggered every frame while pressed
        /// </summary>
        Pressed
    }

    /// <summary>
    /// Handles input devices and converts them to events. Same-frame input ordering is not guaranteed.
    /// Supported devices: Keyboard+Mouse
    /// </summary>
    public class InputEvents : Behaviours.ProtectedSceneSingleton<InputEvents>
    {

        #region Internal Event System
        private readonly Internal.EventSystem<Stick, Vector2> stickEvents = new Internal.EventSystem<Stick, Vector2>();
        private readonly Internal.EventSystem<Axis, float> axisEvents = new Internal.EventSystem<Axis, float>();
        //tuple compares item-wise, so a tuple as a key is valid
        private readonly Internal.EventSystem<(KeyCode keyCode, KeyEvent keyEvent)> buttonEvents = new Internal.EventSystem<(KeyCode keyCode, KeyEvent keyEvent)>();

        protected override void Destroy()
        {
            stickEvents.Clear();
            axisEvents.Clear();
            buttonEvents.Clear();
        }
        #endregion


        // Update is called once per frame
        void Update()
        {
            var modificationProtectesSticks = stickEvents.registry.Keys.ToArray();

            //Loop through all Sticks
            foreach (Stick stick in modificationProtectesSticks)
            {
                //Only invoke events if the stick is valid
                if (stick.IsValidForInput())
                    stickEvents.Handle(stick, stick.GetValue());
            }
            var modifcationProtectedAxes = axisEvents.registry.Keys.ToArray();

            //Loop through all Axis
            foreach (Axis axis in modifcationProtectedAxes)
            {
                //Only invoke events if the axis is valid
                if (axis.IsValidForInput())
                    axisEvents.Handle(axis, axis.GetValue());
            }

            var modifcationProtectedButtons = buttonEvents.registry.Keys.ToArray();
            //Loop through all key/action pairs
            foreach (var keyPair in modifcationProtectedButtons)
            {
                //Determine if it should be invoked based on the key event type
                bool invoke = false;
                switch (keyPair.keyEvent)
                {
                    case KeyEvent.Down:
                        invoke = Input.GetKeyDown(keyPair.keyCode);
                        break;
                    case KeyEvent.Up:
                        invoke = Input.GetKeyUp(keyPair.keyCode);
                        break;
                    case KeyEvent.Pressed:
                        invoke = Input.GetKey(keyPair.keyCode);
                        break;
                }

                //Invoke events if the correct input was detected
                if (invoke)
                    buttonEvents.Handle(keyPair);
            }
        }

        #region Registration Management
        public static void Register(Stick stick, System.Func<Vector2, object> action) => Instance.stickEvents.Register(stick, action);
        public static void Deregister(Stick stick, System.Func<Vector2, object> action) => Instance.stickEvents.Deregister(stick, action);

        public static void Register(Axis axis, System.Func<float, object> action) => Instance.axisEvents.Register(axis, action);
        public static void Deregister(Axis axis, System.Func<float, object> action) => Instance.axisEvents.Deregister(axis, action);

        public static void Register(KeyCode keyCode, KeyEvent keyEvent, System.Func<object> action) => Instance.buttonEvents.Register((keyCode, keyEvent), action);
        public static void Deregister(KeyCode keyCode, KeyEvent keyEvent, System.Func<object> action) => Instance.buttonEvents.Deregister((keyCode, keyEvent), action);
        #endregion

        #region Stick and Axis Implementations
        #region Base
        /// <summary>
        /// Represents a float input, usually within the range of -1.0f to 1.0f, but not guaranteed
        /// </summary>
        public abstract class Axis
        {
            public abstract float GetValue();
            public abstract bool IsValidForInput();
        }

        /// <summary>
        /// Represents a Vector2, usually with x/y in the range of -1.0f to 1.0f, but not guaranteed
        /// </summary>
        public abstract class Stick
        {
            public abstract Vector2 GetValue();
            public abstract bool IsValidForInput();
        }
        #endregion

        #region Custom
        /// <summary>
        /// Creates a psuedo axis from two keys
        /// </summary>
        public class AxisFromKeys : Axis
        {
            /// <summary>
            /// Whether or not this Axis will emit events. Emits events if enabled.
            /// </summary>
            public bool Enabled = false;
            private readonly KeyCode negativeKey, positiveKey;

            public AxisFromKeys(KeyCode negativeKey, KeyCode positiveKey, bool enabled = true)
            {
                this.Enabled = enabled;
                this.negativeKey = negativeKey;
                this.positiveKey = positiveKey;
            }

            public override float GetValue()
            {
                return (Input.GetKey(negativeKey) ? -1.0f : 0.0f) + (Input.GetKey(positiveKey) ? 1.0f : 0.0f);
            }

            public override bool IsValidForInput()
            {
                return Enabled;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                else if (obj.GetType() == typeof(AxisFromKeys))
                {
                    AxisFromKeys other = obj as AxisFromKeys;
                    return negativeKey == other.negativeKey && positiveKey == other.positiveKey;
                }
                else if (obj.GetType() == typeof(System.Tuple<KeyCode, KeyCode>))
                {
                    System.Tuple<KeyCode, KeyCode> other = obj as System.Tuple<KeyCode, KeyCode>;
                    return negativeKey == other.Item1 && positiveKey == other.Item2;
                }
                else
                    return false;
            }

            public override string ToString()
            {
                float negVal = (Input.GetKey(negativeKey) ? -1.0f : 0.0f);
                float posVal = (Input.GetKey(positiveKey) ? 1.0f : 0.0f);
                //Show base components and total
                return string.Format("AxisFromButtons: {0}:{1} + {2}:{3} = {4}",
                    negativeKey.ToString(),
                    negVal,
                    positiveKey.ToString(),
                    posVal,
                    negVal + posVal
                    );
            }

            public override int GetHashCode()
            {
                //Use a tuple of the two keys to get a hash code
                return (negativeKey, positiveKey).GetHashCode();
            }
        }

        /// <summary>
        /// Creates a psuedo stick from two axes
        /// </summary>
        public class StickFromAxes : Stick
        {
            /// <summary>
            /// Whether or not this Stick will emit events. Emits events if enabled.
            /// </summary>
            public bool Enabled = false;
            private readonly Axis xAxis, yAxis;

            public StickFromAxes(Axis xAxis, Axis yAxis, bool enabled = true)
            {
                this.Enabled = enabled;
                this.xAxis = xAxis;
                this.yAxis = yAxis;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                else if (obj.GetType() == typeof(StickFromAxes))
                {
                    StickFromAxes other = obj as StickFromAxes;
                    return xAxis == other.xAxis && yAxis == other.yAxis;
                }
                else if (obj.GetType() == typeof(System.Tuple<Axis, Axis>))
                {
                    System.Tuple<Axis, Axis> other = obj as System.Tuple<Axis, Axis>;
                    return xAxis == other.Item1 && yAxis == other.Item2;
                }
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return (xAxis, yAxis).GetHashCode();
            }

            public override string ToString()
            {
                Vector2 val = GetValue();
                //Show base components and total
                return string.Format("StickFromAxes: {4}\n{0}: {1}\n {2}: {3}",
                    xAxis.ToString(),
                    val.x,
                    yAxis.ToString(),
                    val.y,
                    val.ToString()
                    );
            }

            public override Vector2 GetValue()
            {
                return new Vector2(xAxis.GetValue(), yAxis.GetValue());
            }

            public override bool IsValidForInput()
            {
                return Enabled;
            }
        }

        public class Mouse : Stick
        {

            private static Mouse instance;
            public static Mouse Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Mouse();
                    }
                    return instance;
                }
            }

            private Mouse()
            {
                //Hid constructor
            }

            public static bool eventsOnlyWhenMouseIsInFrame = true;

            public override Vector2 GetValue()
            {
                return Input.mousePosition;
            }

            public override bool Equals(object obj)
            {
                //All instances of mouse are equal
                return obj != null && obj.GetType() == typeof(Mouse);
            }

            public override int GetHashCode()
            {
                //All instances of mouse are equal
                return 0;
            }

            public override bool IsValidForInput()
            {
                /*
                 From the Unity Docs:
                     Input.mousePosition reports the position of the mouse even when it is not inside the Game View, 
                     such as when Cursor.lockState is set to CursorLockMode.None. When running in windowed mode with an unconfined cursor, 
                     position values smaller than 0 or greater than the screen dimensions (Screen.width,Screen.height) indicate that the 
                     mouse cursor is outside of the game window.
                */
                if (eventsOnlyWhenMouseIsInFrame)
                {
                    Vector2 pos = GetValue();
                    return 0 <= pos.x && pos.x <= Screen.width
                        && 0 <= pos.y && pos.y <= Screen.height;
                }
                else
                    return true;
            }
        }

        public class MouseWheel : Axis
        {

            private static MouseWheel instance;
            public static MouseWheel Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new MouseWheel();
                    }
                    return instance;
                }
            }

            private MouseWheel()
            {
                //Hid constructor
            }

            public override float GetValue()
            {
                return Input.mouseScrollDelta.y;
            }

            public override bool IsValidForInput()
            {
                return true;
            }
        }
        #endregion

        #endregion
    }
}