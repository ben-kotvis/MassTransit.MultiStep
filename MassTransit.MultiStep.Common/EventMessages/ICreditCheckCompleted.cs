﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.MultiStep.Common.EventMessages
{
    public interface ICreditCheckCompleted
    {
        Guid SubmissionId { get; set; }
    }
}
