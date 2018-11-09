using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public class ApprovedApprenticeshipMapper : IApprovedApprenticeshipMapper
    {
        private readonly ICurrentDateTime _currentDateTime;

        public ApprovedApprenticeshipMapper(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public ApprovedApprenticeshipViewModel Map(ApprovedApprenticeship source)
        {
            var result = new ApprovedApprenticeshipViewModel
            {
                AccountLegalEntityPublicHashedId = source.AccountLegalEntityPublicHashedId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                ULN = source.ULN,
                DateOfBirth = source.DateOfBirth,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                StopDate = source.StopDate,
                TrainingType = source.TrainingType,
                TrainingName = source.TrainingName,
                PaymentStatus = source.PaymentStatus,
                ProviderName = source.ProviderName,
                EmployerReference = source.EmployerRef,
                CohortReference = source.CohortReference,
                EndpointAssessorName = source.EndpointAssessorName
            };

            //Cost
            result.CurrentCost = source.PriceEpisodes.GetCostOn(_currentDateTime.Now);

            //Pending Change of Circumstances
            switch (source.UpdateOriginator)
            {
                case Originator.Employer:
                    result.PendingChanges = PendingChanges.WaitingForApproval;
                    break;
                case Originator.Provider:
                    result.PendingChanges = PendingChanges.ReadyForApproval;
                    break;
                default:
                    result.PendingChanges = PendingChanges.None;
                    break;
            }

            //Data Locks
            result.PendingDataLockChange = source.DataLocks.Any(x =>
                (x.IsPriceOnly() || x.WithCourseError())
                && x.UnHandled()
                && x.TriageStatus == TriageStatus.Change);

            result.PendingDataLockRestart = source.DataLocks.Any(x =>
                x.WithCourseError()
                && x.UnHandled()
                && x.TriageStatus == TriageStatus.Restart);

            //Statuses
            result.EnableEdit = result.PendingChanges == PendingChanges.None
                                && !result.PendingDataLockChange
                                && !result.PendingDataLockRestart
                                && new[] {PaymentStatus.Active, PaymentStatus.Paused}.Contains(result.PaymentStatus);

            result.CanEditStatus =
                !(new List<PaymentStatus> {PaymentStatus.Completed, PaymentStatus.Withdrawn}).Contains(result
                    .PaymentStatus);

            result.CanEditStopDate =
                (result.PaymentStatus == PaymentStatus.Withdrawn && result.StartDate != result.StopDate);

            result.Status = MapPaymentStatus(source.PaymentStatus, source.StartDate);

            return result;
        }

        private string MapPaymentStatus(PaymentStatus paymentStatus, DateTime? apprenticeshipStartDate)
        {
            var now = new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);
            var waitingToStart = apprenticeshipStartDate.HasValue && apprenticeshipStartDate.Value > now;

            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return waitingToStart ? "Waiting to start" : "Live";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Finished";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }
    }
}