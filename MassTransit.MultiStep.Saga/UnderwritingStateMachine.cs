using Automatonymous;
using MassTransit.MultiStep.Common.EventMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.MultiStep.Saga
{
    internal class UnderwritingStateMachine : MassTransitStateMachine<UnderwritingState>
    {

        public UnderwritingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UnderwritingSubmissionSubmitted, x => x.CorrelateById(context => context.Message.SubmissionId));
            Event(() => CreditCheckCompleted, x => x.CorrelateById(context => context.Message.SubmissionId));
            Event(() => AssesmentRequestCompleted, x => x.CorrelateById(context => context.Message.SubmissionId));

            Schedule(() => ScheduleElapsed, x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(5);
                x.Received = e => e.CorrelateById(context => context.Message.SubmissionId);
            });

            Initially(
                When(UnderwritingSubmissionSubmitted)
                    .Then(context =>
                    {
                        context.Instance.JobRequestId = context.Data.SubmissionId;
                        context.Instance.SubmissionId = context.Data.SubmissionId;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Submission Submitted"))
                    .ThenAsync(context => context.Publish(new UnderwritingSubmissionActivated() { SubmissionId = context.Instance.SubmissionId.Value }))
                    .Schedule(ScheduleElapsed, y => new CheckSubmissionScheduleElapsedEvent() { SubmissionId = y.Instance.SubmissionId.Value }) // Schdule
                    .TransitionTo(Active)
                    );
            During(Active,
                When(CreditCheckCompleted)
                    .Then(context => context.Instance.Tracking |= UnderwritingStateTracking.CreditCheckCompleted)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Tracking is {context.Instance.Tracking}")),
                When(AssesmentRequestCompleted)
                    .Then(context => context.Instance.Tracking |= UnderwritingStateTracking.AssesmentRequestCompleted)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Tracking is {context.Instance.Tracking}")),
                When(ScheduleElapsed.Received)
                .If(new StateMachineCondition<UnderwritingState, ICheckSubmissionScheduleElapsedEvent>(bc =>
                {
                    if ((bc.Instance.Tracking & UnderwritingStateTracking.AssesmentRequestCompleted) != UnderwritingStateTracking.AssesmentRequestCompleted ||
                    (bc.Instance.Tracking & UnderwritingStateTracking.CreditCheckCompleted) != UnderwritingStateTracking.CreditCheckCompleted)
                    {
                        return true;
                    }

                    return false;
                }), x => x
                 .ThenAsync(context => Console.Out.WriteLineAsync("!!!!! Rescheduling....."))
                 .Schedule(ScheduleElapsed, y => new CheckSubmissionScheduleElapsedEvent() { SubmissionId = y.Instance.SubmissionId.Value })) // Reschdule
                .If(new StateMachineCondition<UnderwritingState, ICheckSubmissionScheduleElapsedEvent>(bc =>
                {
                    if ((bc.Instance.Tracking & UnderwritingStateTracking.AssesmentRequestCompleted) == UnderwritingStateTracking.AssesmentRequestCompleted &&
                    (bc.Instance.Tracking & UnderwritingStateTracking.CreditCheckCompleted) == UnderwritingStateTracking.CreditCheckCompleted)
                    {
                        return true;
                    }

                    return false;
                }), x => x
                 .Then(context => context.Instance.Tracking |= UnderwritingStateTracking.ApprovalCompleted)
                 .ThenAsync(context => Console.Out.WriteLineAsync("Completed"))
                 .Finalize())
                );
        }

        public State Active { get; private set; }

        public State Complete { get; private set; }

        #region "Events"
        public Event<IUnderwritingSubmissionSubmitted> UnderwritingSubmissionSubmitted { get; private set; }
        public Event<ICreditCheckCompleted> CreditCheckCompleted { get; private set; }
        public Event<IAssesmentRequestCompleted> AssesmentRequestCompleted { get; private set; }
        #endregion

        #region
        public Schedule<UnderwritingState, ICheckSubmissionScheduleElapsedEvent> ScheduleElapsed { get; private set; }
        #endregion
    }

    public class CheckSubmissionScheduleElapsedEvent: ICheckSubmissionScheduleElapsedEvent
    {

        public Guid SubmissionId { get; set; }
    }

    public class UnderwritingSubmissionActivated : IUnderwritingSubmissionActivated
    {
        public Guid SubmissionId { get; set; }
    }
}
