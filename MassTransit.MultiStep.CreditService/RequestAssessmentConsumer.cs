using MassTransit.MultiStep.Common.EventMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.MultiStep.CreditService
{
    public class RequestAssessmentConsumer : IConsumer<IUnderwritingSubmissionActivated>
    {
        public async Task Consume(ConsumeContext<IUnderwritingSubmissionActivated> context)
        {
            await Console.Out.WriteLineAsync($"!!!!!!!! Request Assesment");

            await context.Publish(new AssesmentRequestCompleted() { SubmissionId = context.Message.SubmissionId });
        }
    }

    public class AssesmentRequestCompleted : IAssesmentRequestCompleted
    {
        public Guid SubmissionId { get; set; }
    }
}
