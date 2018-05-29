using MassTransit.MultiStep.Common.EventMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.MultiStep.CreditService
{
    public class CreditCheckConsumer : IConsumer<IUnderwritingSubmissionActivated>
    {
        public async Task Consume(ConsumeContext<IUnderwritingSubmissionActivated> context)
        {
            await Console.Out.WriteLineAsync($"!!!!!!!! Checking Credit");

            await Task.Delay(5000);

            throw new Exception("Problem occurred");
            //await context.Publish(new CreditCheckCompleted() { SubmissionId = context.Message.SubmissionId });

            
        }
    }

    public class CreditCheckConsumerFault : IConsumer<Fault<IUnderwritingSubmissionActivated>>
    {
        public async Task Consume(ConsumeContext<Fault<IUnderwritingSubmissionActivated>> context)
        {
            await Console.Out.WriteLineAsync($"!!!!!!!! Error occurred");

            await Console.Out.WriteLineAsync(context.Message.Exceptions[0].Message);
        }
    }

    public class CreditCheckCompleted : ICreditCheckCompleted
    {
        public Guid SubmissionId { get; set; }
    }
}
