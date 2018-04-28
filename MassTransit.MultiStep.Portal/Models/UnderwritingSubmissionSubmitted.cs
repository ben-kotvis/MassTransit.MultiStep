using MassTransit.MultiStep.Common.EventMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.MultiStep.Portal.Models
{
    public class UnderwritingSubmissionSubmitted : IUnderwritingSubmissionSubmitted
    {
        public string SubmittedBy { get; set; }
        public Guid SubmissionId { get; set; }
    }
}
