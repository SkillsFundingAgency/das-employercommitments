﻿using System.ComponentModel;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Organisation
{
    public enum EmployerAgreementStatus : short
    {
        [Description("Not signed")]
        Pending = 1,
        [Description("Signed")]
        Signed = 2,
        [Description("Expired")]
        Expired = 3,
        [Description("Superseded")]
        Superseded = 4,
        [Description("Removed")]
        Removed = 5
    }
}
