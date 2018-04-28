using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.MultiStep.Common.Commands
{
    public interface ICheckCreditForSubmission
    {
        Guid SubmissionId { get; set; }
    }
}
