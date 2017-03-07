using System;
using System.Linq.Expressions;
using Szogun1987.EventsAndTasks.Events;

namespace Szogun1987.EventsAndTasks.NonBuilding.Events
{
    public static class EventToTask
    {
        public static ITriggerStep<TContext> Create<TContext>(TContext context)
        {
            return new TriggerStep<TContext>(context);
        }

        public interface ITriggerStep<TContext>
        {
            IResultEventStep<TContext> WithTrigger(Action<TContext> trigger);
        }

        private class TriggerStep<TContext> : ITriggerStep<TContext>
        {
            private readonly TContext _context;

            public TriggerStep(TContext context)
            {
                _context = context;
            }

            public IResultEventStep<TContext> WithTrigger(Action<TContext> trigger)
            {
                return new ResultEventStep<TContext>(_context, trigger);
            }
        }

        public interface IResultEventStep<TContext>
        {
            IFailureEventOrFinalStep<TContext, TResult> WithResultEvent<TResult>(Expression<Action<TContext, EventHandler<TResult>>> subscribeExpression);
        }

        private class ResultEventStep<TContext> : IResultEventStep<TContext>
        {
            private readonly TContext _context;
            private readonly Action<TContext> _trigger;

            public ResultEventStep(TContext context, Action<TContext> trigger)
            {
                _context = context;
                _trigger = trigger;
            }

            public IFailureEventOrFinalStep<TContext, TResult> WithResultEvent<TResult>(Expression<Action<TContext, EventHandler<TResult>>> subscribeExpression)
            {
                var unsubscribeAction = BuildUnsubscribeAction(subscribeExpression);
                var subscribeAction = subscribeExpression.Compile();

                return new FinalStep<TContext, TResult>(
                    _context,
                    _trigger,
                    subscribeAction,
                    unsubscribeAction);
            }
        }

        public interface IFinalStep<TContext, TResult>
        {
            EventToTask<TContext, TResult> Build();
        }

        public interface IFailureEventOrFinalStep<TContext, TResult> : IFinalStep<TContext, TResult>
        {
            IFinalStep<TContext, TResult> WithFailureEvent(Expression<Action<TContext, EventHandler<FailureEventArgs>>> subscribeExpression);
        }

        private class FinalStep<TContext, TResult> : IFailureEventOrFinalStep<TContext, TResult>
        {
            private readonly TContext _context;
            private readonly Action<TContext> _trigger;
            private readonly Action<TContext, EventHandler<TResult>> _resultSubscribeAction;
            private readonly Action<TContext, EventHandler<TResult>> _resultUnsubscribeAction;
            private readonly Action<TContext, EventHandler<FailureEventArgs>> _failureSubscribeAction;
            private readonly Action<TContext, EventHandler<FailureEventArgs>> _failureUnsubscibeAction;


            public FinalStep(
                TContext context,
                Action<TContext> trigger,
                Action<TContext, EventHandler<TResult>> resultSubscribeAction,
                Action<TContext, EventHandler<TResult>> resultUnsubscribeAction)
                : this(context, trigger, resultSubscribeAction, resultUnsubscribeAction, EmptyErrorAction, EmptyErrorAction)
            {

            }

            public FinalStep(
                TContext context,
                Action<TContext> trigger,
                Action<TContext, EventHandler<TResult>> resultSubscribeAction,
                Action<TContext, EventHandler<TResult>> resultUnsubscribeAction,
                Action<TContext, EventHandler<FailureEventArgs>> failureSubscribeAction,
                Action<TContext, EventHandler<FailureEventArgs>> failureUnsubscibeAction)
            {
                _context = context;
                _trigger = trigger;
                _resultSubscribeAction = resultSubscribeAction;
                _resultUnsubscribeAction = resultUnsubscribeAction;
                _failureSubscribeAction = failureSubscribeAction;
                _failureUnsubscibeAction = failureUnsubscibeAction;
            }

            public EventToTask<TContext, TResult> Build()
            {
                return new EventToTask<TContext, TResult>(
                    _context,
                    _resultSubscribeAction,
                    _resultUnsubscribeAction,
                    _failureSubscribeAction,
                    _failureUnsubscibeAction,
                    _trigger);
            }

            public IFinalStep<TContext, TResult> WithFailureEvent(Expression<Action<TContext, EventHandler<FailureEventArgs>>> subscribeExpression)
            {
                var unsubscribeAction = BuildUnsubscribeAction(subscribeExpression);
                var subscribeAction = subscribeExpression.Compile();
                return new FinalStep<TContext, TResult>(
                    _context,
                    _trigger,
                    _resultSubscribeAction,
                    _resultUnsubscribeAction,
                    subscribeAction,
                    unsubscribeAction);
            }

            private static void EmptyErrorAction(TContext context, EventHandler<FailureEventArgs> eventHanlder)
            {

            }
        }

        private static Action<TContext, EventHandler<TResult>> BuildUnsubscribeAction<TContext, TResult>(
            Expression<Action<TContext, EventHandler<TResult>>> subscribeExpression)
        {
            var eventAssignment = subscribeExpression.Body as BinaryExpression;
            if (eventAssignment == null)
            {
                throw new ArgumentException("Event assignment should be passed.", nameof(subscribeExpression));
            }

            if (eventAssignment.NodeType != ExpressionType.AddAssign)
            {
                throw new ArgumentException("Event assignment should be passed.", nameof(subscribeExpression));
            }

            var resultBodyExpression = Expression.SubtractAssign(eventAssignment.Left, eventAssignment.Right);
            var resultExpression = Expression.Lambda(resultBodyExpression, subscribeExpression.Parameters[0],
                subscribeExpression.Parameters[1]);

            return (Action<TContext, EventHandler<TResult>>)resultExpression.Compile();
        }
    }
}