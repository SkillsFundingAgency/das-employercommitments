﻿using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ConfirmationStateChangeViewModel
    {
        public string ApprenticeName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public ChangeStatusViewModel ChangeStatusViewModel { get; set; }
        public string ApprenticeULN { get; set; }
        public string ApprenticeCourse { get; set; }
        public string ViewTransactionsLink { get; set; }
    }
}