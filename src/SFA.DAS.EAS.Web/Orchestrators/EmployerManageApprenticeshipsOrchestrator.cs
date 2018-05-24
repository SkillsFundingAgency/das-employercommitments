﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateProviderPaymentPriority;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;
using ChangeStatusType = SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships.ChangeStatusType;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator : CommitmentsBaseOrchestrator, IEmployerManageApprenticeshipsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ILog _logger;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IApprenticeshipFiltersMapper _apprenticeshipFiltersMapper;

        private readonly IValidateApprovedApprenticeship _approvedApprenticeshipValidator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly ICookieStorageService<UpdateApprenticeshipViewModel>
            _apprenticshipsViewModelCookieStorageService;
        private readonly string _searchPlaceholderText;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-apprentices";

        public EmployerManageApprenticeshipsOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IApprenticeshipMapper apprenticeshipMapper,
            IValidateApprovedApprenticeship approvedApprenticeshipValidator,
            ICurrentDateTime currentDateTime,
            ILog logger,
            ICookieStorageService<UpdateApprenticeshipViewModel> apprenticshipsViewModelCookieStorageService,
            IApprenticeshipFiltersMapper apprenticeshipFiltersMapper,
            IAcademicYearDateProvider academicYearDateProvider,
            IAcademicYearValidator academicYearValidator)
            : base(mediator, hashingService, logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _apprenticeshipMapper = apprenticeshipMapper;
            _currentDateTime = currentDateTime;
            _logger = logger;
            _approvedApprenticeshipValidator = approvedApprenticeshipValidator;
            _apprenticshipsViewModelCookieStorageService = apprenticshipsViewModelCookieStorageService;
            _apprenticeshipFiltersMapper = apprenticeshipFiltersMapper;
            _searchPlaceholderText = "Enter a name";
            _academicYearDateProvider = academicYearDateProvider;
            _academicYearValidator = academicYearValidator;
        }

        public async Task<OrchestratorResponse<ManageApprenticeshipsViewModel>> GetApprenticeships(
            string hashedAccountId, ApprenticeshipFiltersViewModel filters, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting On-programme apprenticeships for empployer: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                if (filters.SearchInput?.Trim() == _searchPlaceholderText.Trim())
                    filters.SearchInput = string.Empty;

                var searchQuery = _apprenticeshipFiltersMapper.MapToApprenticeshipSearchQuery(filters);

                var searchResponse = await _mediator.SendAsync(new ApprenticeshipSearchQueryRequest
                {
                    HashedLegalEntityId = hashedAccountId,
                    Query = searchQuery
                });

                var apprenticeships =
                searchResponse.Apprenticeships
                    .OrderBy(m => m.ApprenticeshipName)
                    .Select(async m => await _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(m, true))
                    .ToList();

                var filterOptions = _apprenticeshipFiltersMapper.Map(searchResponse.Facets);
                filterOptions.SearchInput = searchResponse.SearchKeyword;

                var model = new ManageApprenticeshipsViewModel
                {
                    HashedAccountId = hashedAccountId,
                    Apprenticeships = await Task.WhenAll(apprenticeships),
                    Filters = filterOptions,
                    TotalResults = searchResponse.TotalApprenticeships,
                    PageNumber = searchResponse.PageNumber,
                    TotalPages = searchResponse.TotalPages,
                    TotalApprenticeshipsBeforeFilter = searchResponse.TotalApprenticeshipsBeforeFilter,
                    PageSize = searchResponse.PageSize,
                    SearchInputPlaceholder = _searchPlaceholderText
                };

                return new OrchestratorResponse<ManageApprenticeshipsViewModel>
                {
                    Data = model
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<EditApprenticeshipStopDateViewModel>> GetEditApprenticeshipStopDateViewModel(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(
                    new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                var stopDateEditModel =
                    _apprenticeshipMapper.MapToEditApprenticeshipStopDateViewModel(data.Apprenticeship);

                return new OrchestratorResponse<EditApprenticeshipStopDateViewModel> { Data = stopDateEditModel };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ApprenticeshipDetailsViewModel>> GetApprenticeship(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(
                    new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                var detailsViewModel =
                    await _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(data.Apprenticeship);

                detailsViewModel.PendingDataLockRestart = data.Apprenticeship.DataLockCourseTriaged;
                detailsViewModel.PendingDataLockChange = data.Apprenticeship.DataLockPriceTriaged || data.Apprenticeship.DataLockCourseChangeTriaged;

                return new OrchestratorResponse<ApprenticeshipDetailsViewModel> { Data = detailsViewModel };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeshipForEdit(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Getting Approved Apprenticeship for Editing, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertApprenticeshipStatus(accountId, apprenticeshipId);

                var apprenticeshipData = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = apprenticeshipData.Apprenticeship.CommitmentId
                });

                AssertApprenticeshipIsEditable(apprenticeshipData.Apprenticeship);
                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(apprenticeshipData.Apprenticeship, commitmentData.Commitment);

                apprenticeship.HashedAccountId = hashedAccountId;

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(true)
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetConfirmChangesModel(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId,
            ApprenticeshipViewModel apprenticeship)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertApprenticeshipStatus(accountId, apprenticeshipId);

                var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                var apprenticeships = _apprenticeshipMapper.CompareAndMapToApprenticeshipViewModel(data.Apprenticeship,
                    apprenticeship);

                return new OrchestratorResponse<UpdateApprenticeshipViewModel>
                {
                    Data = await apprenticeships
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetViewChangesViewModel(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(
                async () =>
                {
                    var data = await _mediator.SendAsync(
                        new GetApprenticeshipUpdateRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                    var apprenticeshipResult = await _mediator.SendAsync(
                        new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                    var viewModel = _apprenticeshipMapper.MapFrom(data.ApprenticeshipUpdate);

                    if (viewModel == null)
                    {
                        throw new InvalidStateException("Attempting to update an already updated Apprenticeship");
                    }

                    var apprenticeship = await _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeshipResult.Apprenticeship);
                    viewModel.OriginalApprenticeship = apprenticeship;
                    viewModel.HashedAccountId = hashedAccountId;
                    viewModel.HashedApprenticeshipId = hashedApprenticeshipId;
                    viewModel.ProviderName = apprenticeship.ProviderName;

                    return new OrchestratorResponse<UpdateApprenticeshipViewModel>
                    {
                        Data = viewModel
                    };
                }, hashedAccountId, externalUserId);
        }

        public async Task SubmitUndoApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Undoing pending update for : AccountId {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new UndoApprenticeshipUpdateCommand
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId,
                    UserId = userId,
                    UserDisplayName = userName,
                    UserEmailAddress = userEmail
                });
            }
                , hashedAccountId, userId);
        }

        public async Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship, UpdateApprenticeshipViewModel updatedModel)
        {
            var overlappingErrors = await _mediator.SendAsync(
                    new GetOverlappingApprenticeshipsQueryRequest
                    {
                        Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapFrom(apprenticeship) }
                    });

            var result = _apprenticeshipMapper
                .MapOverlappingErrors(overlappingErrors)
                .ToDictionary(overlap => overlap.Key, overlap => overlap.Value);

            foreach (var error in _approvedApprenticeshipValidator.ValidateToDictionary(apprenticeship))
            {
                result.Add(error.Key, error.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.ValidateAcademicYear(updatedModel))
            {
                result.AddIfNotExists(error.Key, error.Value);
            }

            return result;
        }

        public async Task<OrchestratorResponse<ChangeStatusChoiceViewModel>> GetChangeStatusChoiceNavigation(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)

        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Determining navigation for type of change status selection. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                var isPaused = data.Apprenticeship.PaymentStatus == PaymentStatus.Paused;

                return new OrchestratorResponse<ChangeStatusChoiceViewModel> { Data = new ChangeStatusChoiceViewModel { IsCurrentlyPaused = isPaused } };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<WhenToMakeChangeViewModel>> GetChangeStatusDateOfChangeViewModel(
            string hashedAccountId, string hashedApprenticeshipId,
            ChangeStatusType changeType, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Determining navigation for type of change status selection. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);


                var earliestDate = data.Apprenticeship.StartDate.Value;

                var resuming = changeType == ChangeStatusType.Resume;

                var pausedDate = data.Apprenticeship.PauseDate.HasValue
                    ? data.Apprenticeship.PauseDate.Value
                    : _currentDateTime.Now.Date;

                var mustInvokeAcademicYearFundingRule = _academicYearValidator.Validate(pausedDate) == AcademicYearValidationResult.Success;

                if (resuming && data.Apprenticeship.IsWaitingToStart(_currentDateTime))
                {
                    earliestDate = data.Apprenticeship.PauseDate.Value;
                }
                else if (resuming)
                {
                    earliestDate = mustInvokeAcademicYearFundingRule ? _academicYearDateProvider.CurrentAcademicYearStartDate : data.Apprenticeship.PauseDate.Value;
                }
                else
                {
                    earliestDate = _currentDateTime.Now > _academicYearDateProvider.LastAcademicYearFundingPeriod
                                       && data.Apprenticeship.StartDate.Value < _academicYearDateProvider.CurrentAcademicYearStartDate
                        ? _academicYearDateProvider.CurrentAcademicYearStartDate
                        : data.Apprenticeship.StartDate.Value;
                }

                return new OrchestratorResponse<WhenToMakeChangeViewModel>
                {
                    Data = new WhenToMakeChangeViewModel
                    {
                        EarliestDate = earliestDate,
                        SkipStep = CanChangeDateStepBeSkipped(changeType, data),
                        ChangeStatusViewModel = new ChangeStatusViewModel
                        {
                            ChangeType = changeType
                        },
                        EarliestDateIsStartDate = earliestDate.Equals(data.Apprenticeship.StartDate.Value),
                        ApprenticeshipULN = data.Apprenticeship.ULN,
                        ApprenticeshipName = data.Apprenticeship.ApprenticeshipName,
                        TrainingName = data.Apprenticeship.TrainingName

                    }
                };

            }, hashedAccountId, externalUserId);
        }

        private bool CanChangeDateStepBeSkipped(ChangeStatusType changeType, GetApprenticeshipQueryResponse data)
        {
            return data.Apprenticeship.IsWaitingToStart(_currentDateTime) // Not started
                || data.Apprenticeship.PaymentStatus == PaymentStatus.Paused && changeType == ChangeStatusType.Resume // Resuming 
                || data.Apprenticeship.PaymentStatus == PaymentStatus.Active && changeType == ChangeStatusType.Pause; // Pausing
        }

        public async Task<ValidateWhenToApplyChangeResult> ValidateWhenToApplyChange(string hashedAccountId,
            string hashedApprenticeshipId, ChangeStatusViewModel model)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Validating Date for when to apply change. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}, ChangeType: {model.ChangeType}, ChangeDate: {model.DateOfChange.DateTime}");

            var response = await _mediator.SendAsync(new ValidateStatusChangeDateQuery
            {
                AccountId = accountId,
                ApprenticeshipId = apprenticeshipId,
                ChangeOption = (ChangeOption)model.WhenToMakeChange,
                DateOfChange = model.DateOfChange.DateTime
            });

            return new ValidateWhenToApplyChangeResult
            {
                ValidationResult = response.ValidationResult,
                DateOfChange = response.ValidatedChangeOfDate
            };
        }

        public async Task<OrchestratorResponse<ConfirmationStateChangeViewModel>> GetChangeStatusConfirmationViewModel(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Getting Change Status Confirmation ViewModel. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}, ChangeType: {changeType}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                var result = new OrchestratorResponse<ConfirmationStateChangeViewModel>
                {
                    Data = new ConfirmationStateChangeViewModel
                    {
                        ApprenticeName = data.Apprenticeship.ApprenticeshipName,
                        ApprenticeULN = data.Apprenticeship.ULN,
                        DateOfBirth = data.Apprenticeship.DateOfBirth.Value,
                        ChangeStatusViewModel = new ChangeStatusViewModel
                        {
                            DateOfChange = DetermineChangeDate(changeType, data.Apprenticeship, whenToMakeChange, dateOfChange),
                            ChangeType = changeType,
                            WhenToMakeChange = whenToMakeChange,
                            ChangeConfirmed = false
                        }
                    }
                };

                var notResuming = changeType != ChangeStatusType.Resume;

                if (notResuming) return result;

                result.Data.ChangeStatusViewModel.PauseDate = new DateTimeViewModel(data.Apprenticeship.PauseDate, 90);


                if (data.Apprenticeship.IsWaitingToStart(_currentDateTime))
                {
                    result.Data.ChangeStatusViewModel.DateOfChange = new DateTimeViewModel(_currentDateTime.Now.Date, 90);
                    return result;
                }

                result.Data.ChangeStatusViewModel.DateOfChange = new DateTimeViewModel(data.Apprenticeship.PauseDate.Value, 90);

                var mustInvokeAcademicYearFundingRule = _academicYearValidator.Validate(data.Apprenticeship.PauseDate.Value) == AcademicYearValidationResult.NotWithinFundingPeriod;

                if (!mustInvokeAcademicYearFundingRule) return result;

                result.Data.ChangeStatusViewModel.AcademicYearBreakInTraining = true;

                result.Data.ChangeStatusViewModel.DateOfChange = new DateTimeViewModel(_academicYearDateProvider.CurrentAcademicYearStartDate, 90);


                return result;

            }, hashedAccountId, externalUserId);
        }

        public async Task UpdateStatus(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusViewModel model, string externalUserId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info(
                $"Updating Apprenticeship status to {model.ChangeType}. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                await _mediator.SendAsync(new UpdateApprenticeshipStatusCommand
                {
                    UserId = externalUserId,
                    ApprenticeshipId = apprenticeshipId,
                    EmployerAccountId = accountId,

                    ChangeType = (Domain.Models.Apprenticeship.ChangeStatusType)model.ChangeType,

                    DateOfChange = model.DateOfChange.DateTime.Value,
                    UserEmailAddress = userEmail,
                    UserDisplayName = userName
                });

            }, hashedAccountId, externalUserId);
        }

        public async Task UpdateStopDate(string hashedAccountId, string hashedApprenticeshipId, EditApprenticeshipStopDateViewModel model, string externalUserId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Updating Apprenticeship stop date. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                await _mediator.SendAsync(new UpdateApprenticeshipStopDateCommand
                {
                    UserId = externalUserId,
                    ApprenticeshipId = apprenticeshipId,
                    EmployerAccountId = accountId,
                    NewStopDate = model.NewStopDate.DateTime.Value,
                    UserEmailAddress = userEmail,
                    UserDisplayName = userName,
                    CommitmentId = data.Apprenticeship.CommitmentId
                });

            }, hashedAccountId, externalUserId);
        }

        private void CheckApprenticeshipStateValidForChange(Apprenticeship apprentice)
        {
            if (!IsActiveOrPaused(apprentice))
            {
                throw new InvalidStateException(
                    $"Apprenticeship not is correct state for change: Current:{apprentice.PaymentStatus}");
            }
        }

        private bool IsActiveOrPaused(Apprenticeship apprenticeship)
        {
            return apprenticeship.PaymentStatus != PaymentStatus.Withdrawn ||
                   apprenticeship.PaymentStatus != PaymentStatus.Completed;
        }

        private DateTimeViewModel DetermineChangeDate(ChangeStatusType changeType, Apprenticeship apprenticeship, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange)
        {
            if (changeType == ChangeStatusType.Pause || changeType == ChangeStatusType.Resume)
            {
                return new DateTimeViewModel(_currentDateTime.Now.Date);
            }

            if (apprenticeship.IsWaitingToStart(_currentDateTime))
            {
                return new DateTimeViewModel(apprenticeship.StartDate);
            }

            if (whenToMakeChange == WhenToMakeChangeOptions.Immediately)
            {
                return new DateTimeViewModel(_currentDateTime.Now.Date);
            }

            return new DateTimeViewModel(dateOfChange);
        }

        public async Task CreateApprenticeshipUpdate(UpdateApprenticeshipViewModel apprenticeship, string hashedAccountId, string userId, string userName, string userEmail)
        {
            var employerId = _hashingService.DecodeValue(hashedAccountId);
            await _mediator.SendAsync(new CreateApprenticeshipUpdateCommand
            {
                EmployerId = employerId,
                ApprenticeshipUpdate = _apprenticeshipMapper.MapFrom(apprenticeship),
                UserId = userId,
                UserEmailAddress = userEmail,
                UserDisplayName = userName
            });
        }

        private async Task AssertApprenticeshipStatus(long accountId, long apprenticeshipId)
        {
            var result = await _mediator.SendAsync(new GetApprenticeshipUpdateRequest
            {
                AccountId = accountId,
                ApprenticeshipId = apprenticeshipId
            });

            if (result.ApprenticeshipUpdate != null)
                throw new InvalidStateException($"Pending apprenticeship update, ApprenticeshipId: {apprenticeshipId}");
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>>
            GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(string hashedAccountId,
                string hashedApprenticeshipId)
        {
            var mappedModel = _apprenticshipsViewModelCookieStorageService.Get(CookieName);

            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var apprenticeshipResult = await _mediator.SendAsync(
                new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });
            var apprenticeship = await _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeshipResult.Apprenticeship);
            mappedModel.OriginalApprenticeship = apprenticeship;
            mappedModel.HashedAccountId = hashedAccountId;
            mappedModel.HashedApprenticeshipId = hashedApprenticeshipId;

            return new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = mappedModel };
        }

        public void CreateApprenticeshipViewModelCookie(UpdateApprenticeshipViewModel model)
        {
            _apprenticshipsViewModelCookieStorageService.Delete(CookieName);
            model.OriginalApprenticeship = null;
            _apprenticshipsViewModelCookieStorageService.Create(model, CookieName);
        }

        public async Task SubmitReviewApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, bool isApproved, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new ReviewApprenticeshipUpdateCommand
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId,
                    UserId = userId,
                    IsApproved = isApproved,
                    UserDisplayName = userName,
                    UserEmailAddress = userEmail
                });
            }
                , hashedAccountId, userId);
        }

        private void AssertApprenticeshipIsEditable(Apprenticeship apprenticeship)
        {
            var editable = new[] { PaymentStatus.Active, PaymentStatus.Paused }.Contains(apprenticeship.PaymentStatus);

            if (!editable)
            {
                throw new ValidationException("Unable to edit apprenticeship - status not active or paused");
            }
        }

        public async Task<OrchestratorResponse<PaymentOrderViewModel>> GetPaymentOrder(string hashedAccountId, string user)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            _logger.Trace(
                $"Getting payment order. AccountId: {accountId}");

            return await CheckUserAuthorization(
                async () =>
                {
                    var data = await _mediator.SendAsync(new GetProviderPaymentPriorityRequest { AccountId = accountId });
                    var result = _apprenticeshipMapper.MapPayment(data.Data);

                    if (result.Items == null || result.Items.Count() < 2)
                        return new OrchestratorResponse<PaymentOrderViewModel> { Status = HttpStatusCode.NotFound };

                    return new OrchestratorResponse<PaymentOrderViewModel>
                    {
                        Data = result
                    };
                }, hashedAccountId, user);
        }

        public async Task UpdatePaymentOrder(string hashedAccountId, IEnumerable<long> paymentItems, string user, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            _logger.Trace($"Updating payment order. AccountId: {accountId}");

            await CheckUserAuthorization(
                async () =>
                {
                    await _mediator.SendAsync(new UpdateProviderPaymentPriorityCommand
                    {
                        AccountId = accountId,
                        ProviderPriorityOrder = paymentItems,
                        UserId = user,
                        UserEmailAddress = userEmail,
                        UserDisplayName = userName
                    });

                }, hashedAccountId, user);
        }
    }
}
