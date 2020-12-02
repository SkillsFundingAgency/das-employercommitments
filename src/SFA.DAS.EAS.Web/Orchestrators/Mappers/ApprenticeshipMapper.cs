using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Execution;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly ILinkGenerator _linkGenerator;
        private readonly IFeatureToggleService _featureToggleService;

        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;

        public ApprenticeshipMapper(
            IHashingService hashingService,
            ICurrentDateTime currentDateTime,
            IMediator mediator,
            ILog logger,
            IAcademicYearValidator academicYearValidator,
            IAcademicYearDateProvider academicYearDateProvider,
            ILinkGenerator linkGenerator,
            IFeatureToggleService featureToggleService)
        {
            _hashingService = hashingService;
            _currentDateTime = currentDateTime;
            _mediator = mediator;
            _logger = logger;
            _academicYearValidator = academicYearValidator;
            _academicYearDateProvider = academicYearDateProvider;
            _linkGenerator = linkGenerator;
            _featureToggleService = featureToggleService;
        }

        public ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship, bool disableUlnReuseCheck=false)
        {
            var pendingChange = PendingChanges.None;
            if (apprenticeship.PendingUpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.WaitingForApproval;
            if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.ReadyForApproval;
            
            var statusText = MapPaymentStatus(apprenticeship.PaymentStatus, apprenticeship.StartDate);
            var hashedAccountId = _hashingService.HashValue(apprenticeship.EmployerAccountId);
            var hashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id);
            var pendingChangeOfProviderRequest = apprenticeship.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeProvider && x.Status == ChangeOfPartyRequestStatus.Pending).FirstOrDefault();
            var approvedChangeOfProviderRequest = apprenticeship.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeProvider && x.Status == ChangeOfPartyRequestStatus.Approved).FirstOrDefault();
            var pendingChangeOfEmployerRequest = apprenticeship.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer && x.Status == ChangeOfPartyRequestStatus.Pending).FirstOrDefault();
            var approvedChangeOfEmployerRequest = apprenticeship.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer && x.Status == ChangeOfPartyRequestStatus.Approved).FirstOrDefault();
          
            var result = new ApprenticeshipDetailsViewModel
            {
                HashedApprenticeshipId = hashedApprenticeshipId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.ULN,
                DateOfBirth = apprenticeship.DateOfBirth,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                StopDate = apprenticeship.StopDate,
                PauseDate = apprenticeship.PauseDate,
                CompletionDate = apprenticeship.CompletionDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                PaymentStatus = apprenticeship.PaymentStatus,
                Status = statusText,
                ProviderName = apprenticeship.ProviderName,
                PendingChanges = pendingChange,
                Alerts = MapRecordStatus(apprenticeship.PendingUpdateOriginator,
                    apprenticeship.DataLockCourseTriaged,
                    apprenticeship.DataLockPriceTriaged || apprenticeship.DataLockCourseChangeTriaged),
                EmployerReference = apprenticeship.EmployerRef,
                CohortReference = _hashingService.HashValue(apprenticeship.CommitmentId),
                EnableEdit = pendingChange == PendingChanges.None
                            && !apprenticeship.DataLockCourseTriaged
                            && !apprenticeship.DataLockCourseChangeTriaged
                            && !apprenticeship.DataLockPriceTriaged
                            && new []{ PaymentStatus.Active, PaymentStatus.Paused  }.Contains(apprenticeship.PaymentStatus),
                CanEditStatus = !(new List<PaymentStatus> { PaymentStatus.Completed, PaymentStatus.Withdrawn }).Contains(apprenticeship.PaymentStatus),
                CanEditStopDate = (apprenticeship.PaymentStatus == PaymentStatus.Withdrawn),
                EndpointAssessorName = apprenticeship.EndpointAssessorName,
                TrainingType = apprenticeship.TrainingType,
                ReservationId = apprenticeship.ReservationId,
                MadeRedundant = apprenticeship.MadeRedundant,
                ChangeProviderLink = GetChangeOfProviderLink(hashedAccountId, hashedApprenticeshipId),
                HasPendingChangeOfProviderRequest = pendingChangeOfProviderRequest != null,
                PendingChangeOfProviderRequestWithParty = pendingChangeOfProviderRequest?.WithParty,
                HasApprovedChangeOfProviderRequest = approvedChangeOfProviderRequest != null,
                HashedNewApprenticeshipId = approvedChangeOfProviderRequest?.NewApprenticeshipId != null
                        ? _hashingService.HashValue(approvedChangeOfProviderRequest.NewApprenticeshipId.Value)
                        :null,
                IsContinuation = apprenticeship.ContinuationOfId.HasValue,
                IsChangeOfProviderContinuation = apprenticeship.IsChangeOfProviderContinuation,
                HashedPreviousApprenticeshipId = apprenticeship.ContinuationOfId.HasValue
                        ? _hashingService.HashValue(apprenticeship.ContinuationOfId.Value)
                        : null,
                HasPendingChangeOfEmployerRequest = pendingChangeOfEmployerRequest != null,
                PendingChangeOfEmployerRequestWithParty = pendingChangeOfEmployerRequest?.WithParty,
                HasApprovedChangeOfEmployerRequest = approvedChangeOfEmployerRequest != null
            };

            return result;
        }

        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship, CommitmentView commitment)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);

            //todo: we could have 1 partial, and fold it into edit.cshtml

            var isUpdateLockedForStartDateAndCourse = false;
            var isEndDateLockedForUpdate = false;
            var isLockedForUpdate = false;

            //locking fields concerns post-approval apprenticeships only
            //this method should be split into pre- and post-approval versions
            //ideally dealing with different api types and view models
            if (apprenticeship.PaymentStatus != PaymentStatus.PendingApproval)
            {
                isLockedForUpdate = (!isStartDateInFuture &&
                                     (apprenticeship.HasHadDataLockSuccess || _academicYearValidator.IsAfterLastAcademicYearFundingPeriod &&
                                      apprenticeship.StartDate.HasValue &&
                                      _academicYearValidator.Validate(apprenticeship.StartDate.Value) == AcademicYearValidationResult.NotWithinFundingPeriod))
                                    ||
                                    (commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                                     && apprenticeship.HasHadDataLockSuccess && isStartDateInFuture);

                isUpdateLockedForStartDateAndCourse =
                    commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                    && !apprenticeship.HasHadDataLockSuccess;

                // if editing post-approval, we also lock down end date if...
                //   start date is in the future and has had data lock success
                //   (as the validation rule that disallows setting end date to > current month
                //   means any date entered would be before the start date (which is also disallowed))
                // and open it up if...
                //   data lock success and start date in past
                isEndDateLockedForUpdate = isLockedForUpdate;
                if (commitment.AgreementStatus == AgreementStatus.BothAgreed
                    && apprenticeship.HasHadDataLockSuccess)
                {
                    isEndDateLockedForUpdate = isStartDateInFuture;
                }
            }               

            return new ApprenticeshipViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                HashedCommitmentId = _hashingService.HashValue(apprenticeship.CommitmentId),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                NINumber = apprenticeship.NINumber,
                DateOfBirth =
                    new DateTimeViewModel(apprenticeship.DateOfBirth?.Day, apprenticeship.DateOfBirth?.Month,
                        apprenticeship.DateOfBirth?.Year),
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingCode = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef,
                HasStarted = !isStartDateInFuture,
                IsLockedForUpdate = isLockedForUpdate,
                IsPaidForByTransfer = commitment.TransferSender != null,
                IsInTransferRejectedCohort = commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Rejected,
                IsUpdateLockedForStartDateAndCourse = isUpdateLockedForStartDateAndCourse,
                IsEndDateLockedForUpdate = isEndDateLockedForUpdate,
                ReservationId =  apprenticeship.ReservationId,
                ApprenticeshipEmployerTypeOnApproval = commitment.ApprenticeshipEmployerTypeOnApproval,
                IsContinuation = apprenticeship.ContinuationOfId.HasValue
            };
        }

        public async Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel)
        {
            var apprenticeship = new Apprenticeship
            {
                CommitmentId = _hashingService.DecodeValue(viewModel.HashedCommitmentId),
                Id = string.IsNullOrWhiteSpace(viewModel.HashedApprenticeshipId) ? 0L : _hashingService.DecodeValue(viewModel.HashedApprenticeshipId),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth.DateTime,
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost.AsNullableDecimal(),
                StartDate = viewModel.StartDate.DateTime,
                EndDate = viewModel.EndDate.DateTime,
                ProviderRef = viewModel.ProviderRef,
                EmployerRef = viewModel.EmployerRef
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingCode);

                if(training != null)
                {
                    apprenticeship.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                    apprenticeship.TrainingCode = viewModel.TrainingCode;
                    apprenticeship.TrainingName = training.Title;
                }
                else
                {
                    apprenticeship.TrainingType = viewModel.TrainingType;
                    apprenticeship.TrainingCode = viewModel.TrainingCode;
                    apprenticeship.TrainingName = viewModel.TrainingName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {viewModel.TrainingName}, TrainingCode: {viewModel.TrainingCode}, Employer Ref: {viewModel.EmployerRef}, ApprenticeshipId: {apprenticeship.Id}, Apprenticeship ULN: {viewModel.ULN}");
                }
            }

            return apprenticeship;
        }

        public ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel)
        {
            var apprenticeshipId = _hashingService.DecodeValue(viewModel.HashedApprenticeshipId);
            return new ApprenticeshipUpdate
            {
                ApprenticeshipId =  apprenticeshipId,
                Cost = viewModel.Cost.AsNullableDecimal(),
                DateOfBirth = viewModel.DateOfBirth?.DateTime,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                StartDate = viewModel.StartDate?.DateTime,
                EndDate = viewModel.EndDate?.DateTime,
                Originator = Originator.Employer,
                Status = ApprenticeshipUpdateStatus.Pending,
                TrainingName = viewModel.TrainingName, 
                TrainingCode = viewModel.TrainingCode,
                TrainingType = viewModel.TrainingType,
                EmployerRef = viewModel.EmployerRef
            };
        }

        public UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate)
        {
            return apprenticeshipUpdate == null
                ? null
                : new UpdateApprenticeshipViewModel
                {
                    Cost = NullableDecimalToString(apprenticeshipUpdate.Cost),
                    DateOfBirth = new DateTimeViewModel(apprenticeshipUpdate.DateOfBirth),
                    FirstName = apprenticeshipUpdate.FirstName,
                    LastName = apprenticeshipUpdate.LastName,
                    StartDate = new DateTimeViewModel(apprenticeshipUpdate.StartDate),
                    EndDate = new DateTimeViewModel(apprenticeshipUpdate.EndDate),
                    TrainingName = apprenticeshipUpdate.TrainingName,
                    TrainingCode = apprenticeshipUpdate.TrainingCode,
                    TrainingType = apprenticeshipUpdate.TrainingType,
                    EmployerRef = apprenticeshipUpdate.EmployerRef
                };
        }

        public async Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(
            Apprenticeship original, ApprenticeshipViewModel edited)
        {
            string ChangedOrNull(string a, string edit) => a?.Trim() == edit?.Trim() ? null : edit;

            var apprenticeshipDetailsViewModel = MapToApprenticeshipDetailsViewModel(original);
            var model = new UpdateApprenticeshipViewModel
            {
                FirstName = ChangedOrNull(original.FirstName, edited.FirstName),
                LastName = ChangedOrNull(original.LastName, edited.LastName),
                DateOfBirth = original.DateOfBirth == edited.DateOfBirth.DateTime
                    ? null
                    : edited.DateOfBirth,
                Cost = original.Cost == edited.Cost.AsNullableDecimal() ? null : edited.Cost,
                StartDate =  original.StartDate == edited.StartDate.DateTime
                  ? null
                  : edited.StartDate,
                EndDate = original.EndDate == edited.EndDate.DateTime 
                    ? null
                    : edited.EndDate,
                EmployerRef =  original.EmployerRef?.Trim() == edited.EmployerRef?.Trim()
                            || (string.IsNullOrEmpty(original.EmployerRef)  && string.IsNullOrEmpty(edited.EmployerRef))
                    ? null 
                    : edited.EmployerRef ?? "",
                OriginalApprenticeship = apprenticeshipDetailsViewModel
            };

            if (!string.IsNullOrWhiteSpace(edited.TrainingCode) && original.TrainingCode != edited.TrainingCode)
            {
                var training = await GetTrainingProgramme(edited.TrainingCode);

                if (training != null)
                {
                    model.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                    model.TrainingCode = edited.TrainingCode;
                    model.TrainingName = training.Title;
                }
                else
                {
                    model.TrainingType = edited.TrainingType;
                    model.TrainingCode = edited.TrainingCode;
                    model.TrainingName = edited.TrainingName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {edited.TrainingName}, TrainingCode: {edited.TrainingCode}, Employer Ref: {edited.EmployerRef}, Apprenticeship ULN: {edited.ULN}");
                }
            }

            model.HasHadDataLockSuccess = original.HasHadDataLockSuccess;

            return model;
        }

        public PaymentOrderViewModel MapPayment(IList<ProviderPaymentPriorityItem> data)
        {
            var items = data.Select(m => new PaymentOrderItem
                                 {
                                     ProviderId = m.ProviderId,
                                     ProviderName = m.ProviderName,
                                     Priority = m.PriorityOrder
                                 })
                                 .OrderBy(m => m.ProviderName );

            return new PaymentOrderViewModel { Items = items };
        }

        public IList<PriceChange> MapPriceChanges(IEnumerable<DataLockStatus> dataLocks, List<PriceHistory> history)
        {
            // Only price DLs
            var l = new List<PriceChange>();
            var i = 0;
            foreach (var dl in dataLocks)
            {
                i++;
                var h = history
                    .OrderByDescending(m => m.FromDate)
                    .FirstOrDefault(m => m.FromDate <= dl.IlrEffectiveFromDate);

                l.Add(new PriceChange
                {
                    CurrentStartDate = h?.FromDate ?? DateTime.MinValue,
                    CurrentEndDate = h?.ToDate,
                    CurrentCost = h?.Cost ?? default(decimal),
                    IlrStartDate = dl.IlrEffectiveFromDate ?? DateTime.MinValue,
                    IlrEndDate = dl.IlrEffectiveToDate,
                    IlrCost = dl.IlrTotalCost ?? default(decimal),
                    MissingPriceHistory = h == null
                });
            }

            return l;
        }

        public async Task<IEnumerable<CourseChange>> MapCourseChanges(IEnumerable<DataLockStatus> dataLocks, Apprenticeship apprenticeship, IList<PriceHistory> priceHistory)
        {
            var l = new List<CourseChange>();

            var earliestPriceHistory = priceHistory.Min(x => x.FromDate);

            foreach (var dl in dataLocks.Where(m => m.TriageStatus == TriageStatus.Change))
            {
                var course = new CourseChange
                                 {
                                     CurrentStartDate = earliestPriceHistory,
                                     CurrentEndDate = apprenticeship.EndDate.Value,
                                     CurrentTrainingProgram = apprenticeship.TrainingName,
                                     IlrStartDate = dl.IlrEffectiveFromDate.Value,
                                     IlrEndDate = dl.IlrEffectiveToDate,
                                     IlrTrainingProgram =
                                         (await GetTrainingProgramme(dl.IlrTrainingCourseCode)).Title
                                 };
                l.Add(course);
            }

            return l;
        }

        public EditApprenticeshipStopDateViewModel MapToEditApprenticeshipStopDateViewModel(Apprenticeship apprenticeship)
        {
            var result = new EditApprenticeshipStopDateViewModel
            {
                ApprenticeshipULN = apprenticeship.ULN,
                ApprenticeshipName = apprenticeship.ApprenticeshipName,
                ApprenticeshipHashedId = _hashingService.HashValue(apprenticeship.Id),
                ApprenticeshipStartDate = apprenticeship.StartDate.Value,
                AcademicYearRestriction = _currentDateTime.Now > _academicYearDateProvider.LastAcademicYearFundingPeriod ? //if r14 grace period has past for last a.y.
                    _academicYearDateProvider.CurrentAcademicYearStartDate : default(DateTime?),
                CurrentStopDate = apprenticeship.StopDate.Value,
                NewStopDate = new DateTimeViewModel()
            };

            result.EarliestDate = result.AcademicYearRestriction.HasValue &&
                                  result.AcademicYearRestriction.Value > result.ApprenticeshipStartDate
                ? result.AcademicYearRestriction.Value
                : result.ApprenticeshipStartDate;

            return result;
        }


        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            // TODO: LWA - Need to check is this is called multiple times in a single request.
            var trainingProgrammes = await _mediator.SendAsync(new GetTrainingProgrammesQueryRequest());

            return trainingProgrammes.TrainingProgrammes.FirstOrDefault(x => x.Id == trainingCode);
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? ((int)item).ToString() : string.Empty;
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
                    return "Completed";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private IEnumerable<string> MapRecordStatus(Originator? pendingUpdateOriginator, bool dataLockCourseTriaged, bool changeRequested)
        {
            const string changesPending = "Changes pending";
            const string changesForReview = "Changes for review";
            const string changesRequested = "Changes requested";

            var statuses = new List<string>();

            if (pendingUpdateOriginator != null)
            {
                var t = pendingUpdateOriginator == Originator.Employer 
                    ? changesPending : changesForReview;
                statuses.Add(t);
            }

            if (dataLockCourseTriaged)
                statuses.Add(changesRequested);

            if (changeRequested)
                statuses.Add(changesForReview);

            return statuses.Distinct();
        }

        private string GetChangeOfProviderLink(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (_featureToggleService.Get<ChangeOfProvider>().FeatureEnabled)
            {
                return _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/change-provider");
            }
            else
            {
                return String.Empty;
            }
        }
    }
}