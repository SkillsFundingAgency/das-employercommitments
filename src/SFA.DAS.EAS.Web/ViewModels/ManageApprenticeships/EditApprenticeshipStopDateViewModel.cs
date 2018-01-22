﻿using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class EditApprenticeshipStopDateViewModel
    {
        public string ApprenticeshipULN { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string HashedAccountId { get; set; }

        public DateTime EarliestDate { get; set; }

        public EditStopDateViewModel EditStopDate { get; set; }

        public string ApprenticeshipName { get; set; }

        public DateTime CurrentStopDate { get; set; }
    }
}