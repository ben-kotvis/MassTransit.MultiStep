using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.MultiStep.Common.EventMessages
{
    public interface IUnderwritingSubmissionSubmitted
    {
        string SubmittedBy { get; set; }
        Guid SubmissionId { get; set; }
    }
}
