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

            Initially(
                When(UnderwritingSubmissionSubmitted)
                    .Then(context =>
                    {
                        context.Instance.JobRequestId = context.Data.SubmissionId;
                        context.Instance.SubmissionId = context.Data.SubmissionId;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Submission Submitted"))
                    .ThenAsync(context => context.Publish(new UnderwritingSubmissionActivated() { SubmissionId = context.Instance.SubmissionId.Value }))
                    .TransitionTo(Active)
                    );
            During(Active,
                When(CreditCheckCompleted)
                    .Then(context => context.Instance.Tracking |= UnderwritingStateTracking.CreditCheckCompleted)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Tracking is {context.Instance.Tracking}")),
                When(AssesmentRequestCompleted)
                    .Then(context => context.Instance.Tracking |= UnderwritingStateTracking.AssesmentRequestCompleted)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"!!!!!!!! Tracking is {context.Instance.Tracking}"))
                );
        }

        public State Active { get; private set; }

        public State Complete { get; private set; }

        public Event<IUnderwritingSubmissionSubmitted> UnderwritingSubmissionSubmitted { get; private set; }
        public Event<ICreditCheckCompleted> CreditCheckCompleted { get; private set; }
        public Event<IAssesmentRequestCompleted> AssesmentRequestCompleted { get; private set; }
    }

    public class UnderwritingSubmissionActivated : IUnderwritingSubmissionActivated
    {
        public Guid SubmissionId { get; set; }
    }
}
