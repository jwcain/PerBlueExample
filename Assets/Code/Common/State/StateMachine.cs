using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bestagon.State
{
    public class StateMachine
    {
        protected volatile bool _inTransition = false;

        protected Stack<State> stateStack = new Stack<State>();

        private readonly Coroutine thread = default;

        /// <summary>
        /// Creates a state machine with the provided initial state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static StateMachine CreateMachine<T>() where T : State, new()
        {
            StateMachine m = CreateMachineNoInitalState();
            m.PushState<T>();
            return m;
        }

        public static StateMachine CreateMachineNoInitalState()
        {
            StateMachine m = new StateMachine();
            return m;
        }

        protected StateMachine()
        {
            thread = StateMachineCoroutineBinder.Instance.StartCoroutine(MainHandler());
        }

        public void PushState<T>() where T : State, new()
        {
            State spawned = new T();
            spawned.owner = this;
            stateStack.Push(spawned);
        }
        public void PushState(State existingState)
        {
            existingState.owner = this;
            stateStack.Push(existingState);
        }

        public State Current()
        {
            return stateStack.Count == 0 ? null : stateStack.Peek();
        }

        private bool doTransition = false;
        public void Transition()
        {
            doTransition = true;
        }

        ~StateMachine()
        {
            if (thread != null)
                StateMachineCoroutineBinder.Instance.StopCoroutine(thread);
        }

        private IEnumerator MainHandler()
        {
            while (true)
            {
                if (stateStack.Count == 0)
                    yield return new WaitForEndOfFrame();
                else
                {
                    doTransition = false;
                    State c = stateStack.Pop();
                    yield return c.Enter();

                    while (doTransition == false)
                    {
                        yield return c.Handle();
                        yield return new WaitForEndOfFrame();
                    }

                    yield return c.Exit();
                }
            }
        }

        public class StateMachineCoroutineBinder : Bestagon.Behaviours.SceneSingleton<StateMachineCoroutineBinder>
        {
            protected override void Destroy()
            {
                //throw new System.NotImplementedException();
            }
        }
    }
}