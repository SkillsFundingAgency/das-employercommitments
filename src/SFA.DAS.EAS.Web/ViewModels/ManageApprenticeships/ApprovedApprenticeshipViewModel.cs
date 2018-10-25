using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ApprovedApprenticeshipViewModel
    {
        public string AccountLegalEntityPublicHashedId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ULN { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StopDate { get; set; }

        public TrainingType TrainingType { get; set; }

        public string TrainingName { get; set; }

        public decimal CurrentCost { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string Status { get; set; }

        public string ProviderName { get; set; }

        public PendingChanges PendingChanges { get; set; }

        public bool CanEditStatus { get; set; }

        public string EmployerReference { get; set; }

        public string CohortReference { get; set; }

        public bool EnableEdit { get; set; }

        public bool PendingDataLockRestart { get; set; }

        public bool PendingDataLockChange { get; set; }

        public bool CanEditStopDate { get; set; }

        public string EndpointAssessorName { get; set; }

        public ApprenticeshipFiltersViewModel SearchFiltersForListView { get; set; }
    }
}