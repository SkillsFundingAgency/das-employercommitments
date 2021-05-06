using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipUpdate;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Queries.GetReservationValidation;
using SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.NLog.Logger;
using ChangeStatusType = SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships.ChangeStatusType;
using SFA.DAS.HashingService;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator : CommitmentsBaseOrchestrator, IEmployerManageApprenticeshipsOrchestrator
    {
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly ILinkGenerator _linkGenerator;

        private readonly IValidateApprovedApprenticeship _approvedApprenticeshipValidator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly ICookieStorageService<UpdateApprenticeshipViewModel>
            _apprenticeshipsViewModelCookieStorageService;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-apprentices";

        public EmployerManageApprenticeshipsOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IApprenticeshipMapper apprenticeshipMapper,
            IValidateApprovedApprenticeship approvedApprenticeshipValidator,
            ICurrentDateTime currentDateTime,
            ILog logger,
            ICookieStorageService<UpdateApprenticeshipViewModel> apprenticeshipsViewModelCookieStorageService,
            IAcademicYearDateProvider academicYearDateProvider,
            IAcademicYearValidator academicYearValidator,
            ILinkGenerator linkGenerator)
            : base(mediator, hashingService, logger)
        {
            _apprenticeshipMapper = apprenticeshipMapper;
            _currentDateTime = currentDateTime;
            _approvedApprenticeshipValidator = approvedApprenticeshipValidator;
            _apprenticeshipsViewModelCookieStorageService = apprenticeshipsViewModelCookieStorageService;
            _academicYearDateProvider = academicYearDateProvider;
            _academicYearValidator = academicYearValidator;
            _linkGenerator = linkGenerator;
        }

        public async Task<OrchestratorResponse<EditApprenticeshipStopDateViewModel>> GetEditApprenticeshipStopDateViewModel(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await Mediator.SendAsync(
                    new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                var stopDateEditModel =
                    _apprenticeshipMapper.MapToEditApprenticeshipStopDateViewModel(data.Apprenticeship);
                stopDateEditModel.ApprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details");

                return new OrchestratorResponse<EditApprenticeshipStopDateViewModel> { Data = stopDateEditModel };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ApprenticeshipDetailsViewModel>> GetApprenticeship(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await Mediator.SendAsync(
                    new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                var detailsViewModel =
                    _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(data.Apprenticeship);

                detailsViewModel.PendingDataLockRestart = data.Apprenticeship.DataLockCourseTriaged;
                detailsViewModel.PendingDataLockChange = data.Apprenticeship.DataLockPriceTriaged || data.Apprenticeship.DataLockCourseChangeTriaged;

                return new OrchestratorResponse<ApprenticeshipDetailsViewModel> { Data = detailsViewModel };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeshipForEdit(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info($"Getting Approved Apprenticeship for Editing, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertApprenticeshipStatus(accountId, apprenticeshipId);

                //todo: whenall
                var apprenticeshipData = await Mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                var commitmentData = await Mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = apprenticeshipData.Apprenticeship.CommitmentId
                });

                AssertApprenticeshipIsEditable(apprenticeshipData.Apprenticeship);
                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(apprenticeshipData.Apprenticeship, commitmentData.Commitment);

                apprenticeship.HashedAccountId = hashedAccountId;

                var includeFrameworks = commitmentData.Commitment.ApprenticeshipEmployerTypeOnApproval != ApprenticeshipEmployerType.NonLevy;

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(includeFrameworks)
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetConfirmChangesModel(
            string hashedAccountId, string hashedApprenticeshipId, string externalUserId,
            ApprenticeshipViewModel apprenticeship)
        {
            //todo: could pick up first 2 params from apprenticeship
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertApprenticeshipStatus(accountId, apprenticeshipId);

                var data = await Mediator.SendAsync(new GetApprenticeshipQueryRequest
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
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(
                async () =>
                {
                    var data = await Mediator.SendAsync(
                        new GetApprenticeshipUpdateRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                    var apprenticeshipResult = await Mediator.SendAsync(
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

                    var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeshipResult.Apprenticeship);
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
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Debug($"Undoing pending update for : AccountId {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                await Mediator.SendAsync(new UndoApprenticeshipUpdateCommand
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

        public async Task<IDictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship, UpdateApprenticeshipViewModel updatedModel)
        {
            ConcurrentDictionary<string, string> errors = new ConcurrentDictionary<string, string>();
            await Task.WhenAll(AddOverlapAndDateValidationErrors(errors, apprenticeship, updatedModel), AddReservationValidationErrors(errors, apprenticeship));

            return errors;
        }

        public async Task<OrchestratorResponse<ChangeStatusChoiceViewModel>> GetChangeStatusChoiceNavigation(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Determining navigation for type of change status selection. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                var isPaused = data.Apprenticeship.PaymentStatus == PaymentStatus.Paused;
                var apprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details");            
                
                return new OrchestratorResponse<ChangeStatusChoiceViewModel> { Data = new ChangeStatusChoiceViewModel { IsCurrentlyPaused = isPaused, ApprenticeDetailsV2Link = apprenticeDetailsV2Link } };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<WhenToMakeChangeViewModel>> GetChangeStatusDateOfChangeViewModel(
            string hashedAccountId, string hashedApprenticeshipId,
            ChangeStatusType changeType, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Determining navigation for type of change status selection. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                return new OrchestratorResponse<WhenToMakeChangeViewModel>
                {
                    Data = new WhenToMakeChangeViewModel
                    {
                        ApprenticeStartDate = data.Apprenticeship.StartDate.Value,
                        SkipToConfirmationPage = CanSkipToConfirmationPage(changeType, data),
                        SkipToMadeRedundantQuestion = CanSkipToAskRedundancyQuestion(changeType, data),
                        ChangeStatusViewModel = new ChangeStatusViewModel
                        {
                            ChangeType = changeType
                        },
                        ApprenticeshipULN = data.Apprenticeship.ULN,
                        ApprenticeshipName = data.Apprenticeship.ApprenticeshipName,
                        TrainingName = data.Apprenticeship.TrainingName

                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<RedundantApprenticeViewModel>> GetRedundantViewModel(
            string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, DateTime? dateOfChange, WhenToMakeChangeOptions whenToMakeChange, string externalUserId, bool? madeRedundant)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Determining navigation for type of change status selection. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                return new OrchestratorResponse<RedundantApprenticeViewModel>
                {
                    Data = new RedundantApprenticeViewModel
                    {
                        ApprenticeshipName = data.Apprenticeship.ApprenticeshipName,
                        ChangeStatusViewModel = new ChangeStatusViewModel()
                        {
                            DateOfChange = DetermineChangeDate(changeType, data.Apprenticeship, whenToMakeChange, dateOfChange),
                            ChangeType = changeType,
                            WhenToMakeChange = whenToMakeChange,
                            ChangeConfirmed = false,
                            StartDate = data.Apprenticeship.StartDate,
                            MadeRedundant = madeRedundant
                        }
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        private bool CanSkipToConfirmationPage(ChangeStatusType changeType, GetApprenticeshipQueryResponse data)
        {
            return data.Apprenticeship.PaymentStatus == PaymentStatus.Paused && changeType == ChangeStatusType.Resume // Resuming 
                || data.Apprenticeship.PaymentStatus == PaymentStatus.Active && changeType == ChangeStatusType.Pause; // Pausing
        }

        private bool CanSkipToAskRedundancyQuestion(ChangeStatusType changeType, GetApprenticeshipQueryResponse data)
        {
            return changeType == ChangeStatusType.Stop && data.Apprenticeship.IsWaitingToStart(_currentDateTime);
        }
        public async Task<ValidateWhenToApplyChangeResult> ValidateWhenToApplyChange(string hashedAccountId,
            string hashedApprenticeshipId, ChangeStatusViewModel model)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Validating Date for when to apply change. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}, ChangeType: {model.ChangeType}, ChangeDate: {model.DateOfChange.DateTime}");

            var response = await Mediator.SendAsync(new ValidateStatusChangeDateQuery
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

        public async Task<OrchestratorResponse<ConfirmationStateChangeViewModel>> GetChangeStatusConfirmationViewModel(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange, bool? madeRedundant, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Getting Change Status Confirmation ViewModel. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}, ChangeType: {changeType}");

            return await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
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
                        ApprenticeCourse = data.Apprenticeship.TrainingName,
                        ViewTransactionsLink = _linkGenerator.FinanceLink($"accounts/{hashedAccountId}/finance/{_currentDateTime.Now.Year}/{_currentDateTime.Now.Month}"),

                        ChangeStatusViewModel = new ChangeStatusViewModel
                        {
                            DateOfChange = DetermineChangeDate(changeType, data.Apprenticeship, whenToMakeChange, dateOfChange),
                            ChangeType = changeType,
                            WhenToMakeChange = whenToMakeChange,
                            ChangeConfirmed = false,
                            StartDate = data.Apprenticeship.StartDate,
                            MadeRedundant = madeRedundant
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
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info(
                $"Updating Apprenticeship status to {model.ChangeType}. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                CheckApprenticeshipStateValidForChange(data.Apprenticeship);

                await Mediator.SendAsync(new UpdateApprenticeshipStatusCommand
                {
                    UserId = externalUserId,
                    ApprenticeshipId = apprenticeshipId,
                    EmployerAccountId = accountId,
                    ChangeType = (Domain.Models.Apprenticeship.ChangeStatusType)model.ChangeType,
                    DateOfChange = model.DateOfChange.DateTime.Value,
                    UserEmailAddress = userEmail,
                    UserDisplayName = userName,
                    MadeRedundant = model.MadeRedundant
                });

            }, hashedAccountId, externalUserId);
        }

        public async Task UpdateStopDate(string hashedAccountId, string hashedApprenticeshipId, EditApprenticeshipStopDateViewModel model, string externalUserId, string userName, string userEmail)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            Logger.Info($"Updating Apprenticeship stop date. AccountId: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                var data =
                    await
                        Mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                await Mediator.SendAsync(new UpdateApprenticeshipStopDateCommand
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
            var employerId = HashingService.DecodeValue(hashedAccountId);
            await Mediator.SendAsync(new CreateApprenticeshipUpdateCommand
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
            var result = await Mediator.SendAsync(new GetApprenticeshipUpdateRequest
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
            var mappedModel = _apprenticeshipsViewModelCookieStorageService.Get(CookieName);

            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);
            var accountId = HashingService.DecodeValue(hashedAccountId);

            var apprenticeshipResult = await Mediator.SendAsync(
                new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });
            var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(apprenticeshipResult.Apprenticeship);
            mappedModel.OriginalApprenticeship = apprenticeship;
            mappedModel.HashedAccountId = hashedAccountId;
            mappedModel.HashedApprenticeshipId = hashedApprenticeshipId;

            return new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = mappedModel };
        }

        public void CreateApprenticeshipViewModelCookie(UpdateApprenticeshipViewModel model)
        {
            _apprenticeshipsViewModelCookieStorageService.Delete(CookieName);
            model.OriginalApprenticeship = null;
            _apprenticeshipsViewModelCookieStorageService.Create(model, CookieName);
        }

        public async Task SubmitReviewApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, bool isApproved, string userName, string userEmail)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            await CheckUserAuthorization(async () =>
            {
                await Mediator.SendAsync(new ReviewApprenticeshipUpdateCommand
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

        private async Task AddOverlapAndDateValidationErrors(ConcurrentDictionary<string, string> errors, ApprenticeshipViewModel apprenticeship, UpdateApprenticeshipViewModel updatedModel)
        {
            void AddToErrors(IDictionary<string, string> items)
            {
                foreach (var error in items)
                {
                    errors.TryAdd(error.Key, error.Value);
                }
            }

            var overlappingErrors = await Mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapFrom(apprenticeship) }
                });

            AddToErrors(_approvedApprenticeshipValidator.MapOverlappingErrors(overlappingErrors));
            AddToErrors(_approvedApprenticeshipValidator.ValidateToDictionary(apprenticeship));
            AddToErrors(_approvedApprenticeshipValidator.ValidateAcademicYear(updatedModel));
        }

        private async Task AddReservationValidationErrors(ConcurrentDictionary<string, string> errors, ApprenticeshipViewModel model)
        {
            void AddToErrors(ReservationValidationResult data)
            {
                foreach (var error in data.ValidationErrors)
                {
                    errors.TryAdd(error.PropertyName, error.Reason);
                }
            }

            if (model.ReservationId == null)
            {
                Logger.Info($"Apprenticeship: {HashingService.DecodeValue(model.HashedApprenticeshipId)} Reservation-id:null - no reservation validation required");
                return;
            }

            if (model.StartDate?.DateTime == null)
            {
                Logger.Info($"Apprenticeship: {HashingService.DecodeValue(model.HashedApprenticeshipId)} Reservation-id:{model.ReservationId} start date required for reservation validation");
                return;
            }

            var response = await Mediator.SendAsync(
                    new GetReservationValidationRequest
                    {
                        StartDate = model.StartDate.DateTime.Value,
                        TrainingCode = model.TrainingCode,
                        ReservationId = model.ReservationId.Value
                    }
                );

            AddToErrors(response.Data);
        }
    }
}
