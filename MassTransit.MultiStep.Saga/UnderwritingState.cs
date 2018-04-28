using Automatonymous;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.MultiStep.Saga
{
    internal class UnderwritingState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid? JobRequestId { get; set; }

        public Guid? SubmissionId { get; set; }

        public UnderwritingStateTracking Tracking { get; set; }

        public Guid? ExpirationId { get; set; }
    }

    [Flags]
    internal enum UnderwritingStateTracking
    {
        None = 0,
        CreditCheckCompleted = 1,
        AssesmentRequestCompleted = 2,
        ApprovalCompleted = 4
    }
}
