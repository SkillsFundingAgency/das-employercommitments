using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public sealed class EditApprenticeshipStopDateViewModel : ViewModelBase
    {
        public string ApprenticeshipULN { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string HashedAccountId { get; set; }

        public EditStopDateViewModel EditStopDate { get; set; }

        public string ApprenticeshipName { get; set; }

        
        public DateTime EarliestDate { get; set; }

        public DateTime CurrentStopDate { get; set; }

        public bool EarliestDateIsStartDate { get; set; }
    }
}