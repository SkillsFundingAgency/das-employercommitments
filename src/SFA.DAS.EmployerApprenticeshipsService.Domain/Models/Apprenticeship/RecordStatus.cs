﻿using System.ComponentModel;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship
{
    public enum RecordStatus
    {
        [Description("No action needed")]
        NoActionNeeded,
        [Description("Changes pending")]
        ChangesPending,
        [Description("Changes for review")]
        ChangesForReview,
        [Description("Change requested")]
        ChangeRequested,
    }
}
