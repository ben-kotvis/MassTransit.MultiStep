using MassTransit.MultiStep.Common.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.MultiStep.CreditService
{
    public class CreditCheckConsumer : IConsumer<ICheckCreditForSubmission>
    {
        public async Task Consume(ConsumeContext<ICheckCreditForSubmission> context)
        {
            await Console.Out.WriteLineAsync($"Checking Credit");
        }
    }
}
