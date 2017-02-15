using System;
using System.Collections.Generic;

namespace Szogun1987.EventsAndTasks.Promises
{
    /// <summary>
    /// It is not production-ready implementation of promise
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SamplePromise<T> : IPromise<T>
    {
        private PromiseState _state;

        public SamplePromise()
        {
            _state = new FuturePromise();
        }

        private abstract class PromiseState
        {
            public abstract PromiseState Brake(Exception exception);

            public abstract PromiseState FulFill(T result);
            
            public abstract void OnDone(Action<T> action);
            public abstract void OnError(Action<Exception> action);
            public abstract void Always(Action action);
        }


        public void Brake(Exception exception)
        {
            _state = _state.Brake(exception);
        }

        public void FulFill(T result)
        {
            _state = _state.FulFill(result);
        }

        public IPromise<T> OnDone(Action<T> action)
        {
            _state.OnDone(action);
            return this;
        }

        public IPromise<T> OnError(Action<Exception> action)
        {
            _state.OnError(action);
            return this;
        }

        public IPromise<T> Always(Action action)
        {
            _state.Always(action);
            return this;
        }

        private class FuturePromise : PromiseState
        {
            private readonly List<Action<T>> _doneActions;
            private readonly List<Action<Exception>> _failActions;
            private readonly List<Action> _alwaysActions;

            public FuturePromise()
            {
                _doneActions = new List<Action<T>>();
                _failActions = new List<Action<Exception>>();
                _alwaysActions = new List<Action>();
            }

            public override PromiseState Brake(Exception exception)
            {
                foreach (var failAction in _failActions)
                {
                    failAction(exception);
                }
                Complete();
                return new BrokenState(exception);
            }

            public override PromiseState FulFill(T result)
            {
                foreach (var doneAction in _doneActions)
                {
                    doneAction(result);
                }

                Complete();
                return new FulFilledState(result);
            }

            public override void OnDone(Action<T> action)
            {
                _doneActions.Add(action);
            }

            public override void OnError(Action<Exception> action)
            {
                _failActions.Add(action);
            }

            public override void Always(Action action)
            {
                _alwaysActions.Add(action);
            }

            private void Complete()
            {
                _failActions.Clear();
                _doneActions.Clear();

                foreach (var alwaysAction in _alwaysActions)
                {
                    alwaysAction();
                }
            }
        }

        private class FulFilledState : PromiseState
        {
            private readonly T _result;

            public FulFilledState(T result)
            {
                _result = result;
            }

            public override PromiseState Brake(Exception exception)
            {
                throw new InvalidOperationException("Promise already fulfilled");
            }

            public override PromiseState FulFill(T result)
            {
                throw new InvalidOperationException("Promise already fulfilled");
            }

            public override void OnDone(Action<T> action)
            {
                action(_result);
            }

            public override void OnError(Action<Exception> action)
            {
            }

            public override void Always(Action action)
            {
                action();
            }
        }

        private class BrokenState : PromiseState
        {
            private readonly Exception _exception;

            public BrokenState(Exception exception)
            {
                _exception = exception;
            }
            
            public override PromiseState Brake(Exception exception)
            {
                throw new InvalidOperationException("Promise already broken");
            }

            public override PromiseState FulFill(T result)
            {
                throw new InvalidOperationException("Promise already broken");
            }

            public override void OnDone(Action<T> action)
            {
            }

            public override void OnError(Action<Exception> action)
            {
                action(_exception);
            }

            public override void Always(Action action)
            {
                action();
            }
        }

    }
}
