﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        private readonly IAcademicYearValidator _academicYearValidator;

        public ApprenticeshipMapper(
            IHashingService hashingService,
            ICurrentDateTime currentDateTime,
            IMediator mediator,
            ILog logger,
            IAcademicYearValidator academicYearValidator)
        {
            if (hashingService == null) throw new ArgumentNullException(nameof(hashingService));
            if (currentDateTime == null) throw new ArgumentNullException(nameof(currentDateTime));
            if (mediator == null) throw new ArgumentNullException(nameof(mediator));
            if (academicYearValidator== null) throw new ArgumentNullException(nameof(academicYearValidator));

            _hashingService = hashingService;
            _currentDateTime = currentDateTime;
            _mediator = mediator;
            _logger = logger;
            _academicYearValidator = academicYearValidator;
        }

        public ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship)
        {
            var pendingChange = PendingChanges.None;
            if (apprenticeship.PendingUpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.WaitingForApproval;
            if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.ReadyForApproval;
            
            var statusText = MapPaymentStatus(apprenticeship.PaymentStatus, apprenticeship.StartDate);

            return new ApprenticeshipDetailsViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = statusText,
                ProviderName = apprenticeship.ProviderName,
                PendingChanges = pendingChange,
                Alerts = MapRecordStatus(apprenticeship.PendingUpdateOriginator, 
                    apprenticeship.DataLockCourseTriaged, 
                    apprenticeship.DataLockPriceTriaged),
                EmployerReference = apprenticeship.EmployerRef,
                CohortReference = _hashingService.HashValue(apprenticeship.CommitmentId),
                EnableEdit = pendingChange == PendingChanges.None
                            && !apprenticeship.DataLockCourseTriaged
                            && !apprenticeship.DataLockPriceTriaged
                            && new []{ PaymentStatus.Active, PaymentStatus.Paused,  }.Contains(apprenticeship.PaymentStatus),
                CanEditStatus = !(new List<PaymentStatus> { PaymentStatus.Completed, PaymentStatus.Withdrawn }).Contains(apprenticeship.PaymentStatus)
            };
        }

        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship, IEnumerable<DataLockStatus> dataLocks)
        {
            var a = MapToApprenticeshipViewModel(apprenticeship);
            a.IsLockedForUpdated = dataLocks.Any(m => m.ErrorCode == DataLockErrorCode.None);

            if (_academicYearValidator .IsAfterLastAcademicYearFundingPeriod &&
                 a.StartDate.DateTime.HasValue && 
                 _academicYearValidator.Validate(a.StartDate.DateTime.Value) == AcademicYearValidationResult.NotWithinFundingPeriod)
            {
                a.IsLockedForUpdated = true;
            }
            
            return a;
        }

        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);

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
                IsLockedForUpdated = false
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

        public Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
        {
            var dict = new Dictionary<string, string>();
            const string StartText = "The start date is not valid";
            const string EndText = "The end date is not valid";

            const string StartDateKey = "StartDateOverlap";
            const string EndDateKey = "EndDateOverlap";


            foreach (var item in overlappingErrors.GetFirstOverlappingApprenticeships())
            {
                switch (item.ValidationFailReason)
                {
                    case ValidationFailReason.OverlappingStartDate:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        break;
                    case ValidationFailReason.OverlappingEndDate:
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateEmbrace:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateWithin:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                }
            }
            return dict;
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
            return new UpdateApprenticeshipViewModel
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
            Func<string, string, string> changedOrNull = (a, edit) => 
                a?.Trim() == edit?.Trim() ? null : edit;

            var apprenticeshipDetailsViewModel = MapToApprenticeshipDetailsViewModel(original);
            var model = new UpdateApprenticeshipViewModel
            {
                FirstName = changedOrNull(original.FirstName, edited.FirstName),
                LastName = changedOrNull(original.LastName, edited.LastName),
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
                    Title = $"Change {i}",
                    CurrentStartDate = h?.FromDate ?? DateTime.MinValue,
                    CurrentCost = h?.Cost ?? default(decimal),
                    IlrStartDate = dl.IlrEffectiveFromDate ?? DateTime.MinValue,
                    IlrCost = dl.IlrTotalCost ?? default(decimal),
                    MissingPriceHistory = h == null
                });
            }

            return l;
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
                    return "Finished";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private IEnumerable<string> MapRecordStatus(Originator? pendingUpdateOriginator, bool dataLockCourseTriaged, bool dataLockPriceTriaged)
        {
            const string ChangesPending = "Changes pending";
            const string ChangesForReview = "Changes for review";
            const string ChangesRequested = "Changes requested";

            var statuses = new List<string>();

            if (pendingUpdateOriginator != null)
            {
                var t = pendingUpdateOriginator == Originator.Employer 
                    ? ChangesPending : ChangesForReview;
                statuses.Add(t);
            }

            if (dataLockCourseTriaged)
                statuses.Add(ChangesRequested);

            if (dataLockPriceTriaged)
                statuses.Add(ChangesForReview);

            return statuses.Distinct();
        }
    }
}