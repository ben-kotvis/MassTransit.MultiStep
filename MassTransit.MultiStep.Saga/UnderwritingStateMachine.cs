using Automatonymous;
using MassTransit.MultiStep.Common.Commands;
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

            Initially(
                When(UnderwritingSubmissionSubmitted)
                    .Then(context =>
                    {
                        context.Instance.JobRequestId = context.Data.SubmissionId;
                        context.Instance.SubmissionId = context.Data.SubmissionId;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Submission Submitted"))
                    .ThenAsync(context => context.Publish(new Credit() { SubmissionId = context.Instance.SubmissionId.Value }))
                    .TransitionTo(Active)
                    .Finalize()
                    );
        }

        public State Active { get; private set; }

        public State Complete { get; private set; }

        public Event<IUnderwritingSubmissionSubmitted> UnderwritingSubmissionSubmitted { get; private set; }
    }

    public class Credit : ICheckCreditForSubmission
    {
        public Guid SubmissionId { get; set; }
    }
}
