using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public sealed class WhenToMakeChangeViewModel
    {
        public bool SkipToMadeRedundantQuestion { get; set; }
        public bool SkipToConfirmationPage { get; set; }
        public ChangeStatusViewModel ChangeStatusViewModel { get; set; }
        public DateTime ApprenticeStartDate { get; set; }
        public string ApprenticeshipULN { get; set; }
        public string ApprenticeshipName { get; set; }
        public string TrainingName { get; set; }
    }
}