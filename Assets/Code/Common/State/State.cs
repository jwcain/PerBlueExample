using System.Collections;
namespace Bestagon.State
{
    public abstract class State
    {
        public StateMachine owner;
        public abstract IEnumerator Enter();
        public abstract IEnumerator Exit();
        public abstract IEnumerator Handle();

        protected void PushState<T>() where T : State, new()
        {
            owner.PushState<T>();
        }
        protected void PushState(State existingState)
        {
            owner.PushState(existingState);
        }
        protected void PushAndReturn<T>() where T : State, new()
        {
            owner.PushState(this);
            owner.PushState<T>();
        }
        protected void PushAndReturn(State existingState)
        {
            owner.PushState(this);
            owner.PushState(existingState);
        }

        protected void Transition()
        {
            owner.Transition();
        }


        protected void TransitionWithPush<T>() where T : State, new()
        {
            PushState<T>();
            Transition();
        }
        protected void TransitionWithPush(State existingState)
        {
            PushState(existingState);
            Transition();
        }

        protected void TransitionWithPushAndReturn<T>() where T : State, new()
        {
            PushAndReturn<T>();
            Transition();
        }
        protected void TransitionWithPushAndReturn(State existingState)
        {
            PushAndReturn(existingState);
            Transition();
        }


    }
}