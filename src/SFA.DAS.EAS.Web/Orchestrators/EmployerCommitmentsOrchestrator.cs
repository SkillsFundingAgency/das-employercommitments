using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Application.Commands.DeleteApprentice;
using SFA.DAS.EmployerCommitments.Application.Commands.DeleteCommitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SubmitCommitment;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProvider;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.NLog.Logger;

using OrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.HashingService;
using CallerType = SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment.CallerType;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class EmployerCommitmentsOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;
        private readonly ILog _logger;

        private readonly Func<int, string> _addPluralizationSuffix = i => i > 1 ? "s" : "";
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ICommitmentMapper _commitmentMapper;
        private readonly IFeatureToggleService _featureToggleService;

        public EmployerCommitmentsOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IPublicHashingService publicHashingService,
            IApprenticeshipMapper apprenticeshipMapper,
            ICommitmentMapper commitmentMapper,
            ILog logger,
            IFeatureToggleService featureToggleService) : base(mediator, hashingService, logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
            _apprenticeshipMapper = apprenticeshipMapper;
            _commitmentMapper = commitmentMapper;
            _logger = logger;
            _featureToggleService = featureToggleService;
        }

        public async Task<OrchestratorResponse<CommitmentsIndexViewModel>> GetIndexViewModel(string hashedAccountId, string externalUserId)
        {
            return await CheckUserAuthorization(async () =>
            {
                var accountId = _hashingService.DecodeValue(hashedAccountId);

                var response = await _mediator.SendAsync(new GetProviderPaymentPriorityRequest { AccountId = accountId });

                return new OrchestratorResponse<CommitmentsIndexViewModel>
                {
                    Data = new CommitmentsIndexViewModel
                    {
                        ShowSetPaymentPriorityLink = response.Data != null && response.Data.Count > 1,
                        ShowPublicSectorReportingLink = _featureToggleService.Get<PublicSectorReporting>().FeatureEnabled
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentInformViewModel>> GetInform(string hashedAccountId, string externalUserId)
        {
            return await CheckUserAuthorization(() =>
            {
                return Task.FromResult(new OrchestratorResponse<CommitmentInformViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new CommitmentInformViewModel
                    {
                        HashedAccountId = hashedAccountId
                    }
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SelectProviderViewModel>> GetProviderSearch(string hashedAccountId,
            string externalUserId, string transferConnectionCode, string legalEntityCode, string cohortRef)
        {
            return await CheckUserAuthorization(() =>
            {
                return Task.FromResult(new OrchestratorResponse<SelectProviderViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new SelectProviderViewModel
                    {
                        TransferConnectionCode = transferConnectionCode,
                        LegalEntityCode = legalEntityCode,
                        CohortRef = cohortRef
                    }
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SelectLegalEntityViewModel>> GetLegalEntities(string hashedAccountId, string transferConnectionCode, string cohortRef, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting list of Legal Entities for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId,
                    SignedOnly = false
                });

                return new OrchestratorResponse<SelectLegalEntityViewModel>
                {
                    Data = new SelectLegalEntityViewModel
                    {
                        TransferConnectionCode = transferConnectionCode,
                        CohortRef = string.IsNullOrWhiteSpace(cohortRef) ? CreateReference() : cohortRef,
                        LegalEntities = legalEntities.LegalEntities
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SelectTransferConnectionViewModel>> GetTransferConnections(
            string hashedAccountId, string externalUserId)
        {
            if (!_featureToggleService.Get<Transfers>().FeatureEnabled)
            {
                return new OrchestratorResponse<SelectTransferConnectionViewModel>
                {
                    Data = new SelectTransferConnectionViewModel
                    {
                        TransferConnections = new List<TransferConnectionViewModel>()
                    }
                };
            }

            return await CheckUserAuthorization(async () =>
            {
                var response = await GetTransferConnectionsNoAuthorizationCheck(hashedAccountId, externalUserId);

                return new OrchestratorResponse<SelectTransferConnectionViewModel>
                {
                    Data = new SelectTransferConnectionViewModel
                    {
                        TransferConnections = _commitmentMapper.MapToTransferConnectionsViewModel(response.TransferConnections)
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        private Task<GetAccountTransferConnectionsResponse> GetTransferConnectionsNoAuthorizationCheck(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting list of Transferring Entities for Account: {accountId}");

            return _mediator.SendAsync(new GetAccountTransferConnectionsRequest
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });
        }

        public async Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId, SelectProviderViewModel model)
        {
            var providerId = int.Parse(model.ProviderId);
            return await GetProvider(hashedAccountId, externalUserId, providerId, model.TransferConnectionCode,  model.LegalEntityCode, model.CohortRef);
        }

        public async Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId, ConfirmProviderViewModel model)
        {
            return await GetProvider(hashedAccountId, externalUserId, model.ProviderId, model.TransferConnectionCode, model.LegalEntityCode, model.CohortRef);
        }

        private Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId,
            string externalUserId, int providerId, string transferConnectionCode, string legalEntityCode,
            string cohortRef)
        {
            _logger.Info($"Getting Provider Details, Provider: {providerId}");

            return CheckUserAuthorization(async () =>
            {
                var provider = await ProviderSearch(providerId);

                return new OrchestratorResponse<ConfirmProviderViewModel>
                {
                    Data = new ConfirmProviderViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        TransferConnectionCode = transferConnectionCode,
                        LegalEntityCode = legalEntityCode,
                        ProviderId = providerId,
                        Provider = provider,
                        CohortRef = cohortRef,
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        private async Task<Provider> ProviderSearch(long providerId)
        {
            var response = await _mediator.SendAsync(new GetProviderQueryRequest
            {
                ProviderId = providerId
            });

            return response.ProvidersView?.Provider;
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId,
            string transferConnectionCode, string legalEntityCode, string providerId, string cohortRef,
            string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting Commitment Summary Model for Account: {accountId}, LegalEntity: {legalEntityCode}, Provider: {providerId}");

            return await CheckUserAuthorization(async () =>
            {
                var provider = await ProviderSearch(int.Parse(providerId));

                var legalEntities = await GetActiveLegalEntities(hashedAccountId, externalUserId);
                var legalEntity = legalEntities.LegalEntities.Single(x =>
                    x.Code.Equals(legalEntityCode, StringComparison.InvariantCultureIgnoreCase));

                return new OrchestratorResponse<CreateCommitmentViewModel>
                {
                    Data = new CreateCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        TransferConnectionCode = transferConnectionCode,
                        LegalEntityCode = legalEntityCode,
                        LegalEntityName = legalEntity.Name,
                        LegalEntityAddress = legalEntity.RegisteredAddress,
                        LegalEntitySource = legalEntity.Source,
                        ProviderId = provider.Ukprn,
                        ProviderName = provider.ProviderName,
                        CohortRef = cohortRef
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<string>> CreateEmployerAssignedCommitment(CreateCommitmentViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Employer assigned commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            return await CheckUserAuthorization(async () =>
            {
                (long? transferSenderId, string transferSenderName) = await GetTransferConnectionInfo(model.HashedAccountId, model.TransferConnectionCode, externalUserId);

                var response = await _mediator.SendAsync(new CreateCommitmentCommand
                {
                    Commitment = new Commitment
                    {
                        Reference = model.CohortRef,
                        EmployerAccountId = accountId,
                        TransferSenderId = transferSenderId,
                        TransferSenderName = transferSenderName,
                        LegalEntityId = model.LegalEntityCode,
                        LegalEntityName = model.LegalEntityName,
                        LegalEntityAddress = model.LegalEntityAddress,
                        LegalEntityOrganisationType = (OrganisationType)model.LegalEntitySource,
                        ProviderId = model.ProviderId,
                        ProviderName = model.ProviderName,
                        CommitmentStatus = CommitmentStatus.New,
                        EditStatus = EditStatus.EmployerOnly,
                        EmployerLastUpdateInfo = new LastUpdateInfo { Name = userDisplayName, EmailAddress = userEmail }
                    },
                    UserId = externalUserId
                });

                return new OrchestratorResponse<string>
                {
                    Data = _hashingService.HashValue(response.CommitmentId)
                };

            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<string>> CreateProviderAssignedCommitment(SubmitCommitmentViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Provider assigned Commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            return await CheckUserAuthorization(async () =>
            {
                (long? transferSenderId, string transferSenderName) = await GetTransferConnectionInfo(model.HashedAccountId, model.TransferConnectionCode, externalUserId);

                var response = await _mediator.SendAsync(new CreateCommitmentCommand
                {
                    Message = model.Message,
                    Commitment = new Commitment
                    {
                        Reference = model.CohortRef,
                        EmployerAccountId = accountId,
                        TransferSenderId = transferSenderId,
                        TransferSenderName = transferSenderName,
                        LegalEntityId = model.LegalEntityCode,
                        LegalEntityName = model.LegalEntityName,
                        LegalEntityAddress = model.LegalEntityAddress,
                        LegalEntityOrganisationType = (OrganisationType)model.LegalEntitySource,
                        ProviderId = long.Parse(model.ProviderId),
                        ProviderName = model.ProviderName,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.ProviderOnly,
                        EmployerLastUpdateInfo = new LastUpdateInfo { Name = userDisplayName, EmailAddress = userEmail },
                    },
                    UserId = externalUserId
                });

                return new OrchestratorResponse<string>
                {
                    Data = _hashingService.HashValue(response.CommitmentId)
                };

            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetSkeletonApprenticeshipDetails(string hashedAccountId, string externalUserId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting skeleton apprenticeship model, Account: {accountId}, Commitment: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                CheckCommitmentIsVisibleToEmployer(commitmentData.Commitment);

                var apprenticeship = new ApprenticeshipViewModel
                {
                    HashedAccountId = hashedAccountId,
                    HashedCommitmentId = hashedCommitmentId,
                    IsPaidForByTransfer = commitmentData.Commitment.TransferSender != null,
                    IsInTransferRejectedCohort = commitmentData.Commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Rejected
                };

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(commitmentData.Commitment.TransferSender == null)
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship, string externalUserId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var commitmentId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            _logger.Info($"Creating Apprenticeship, Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () =>
            {
                await CheckCommitmentIsVisibleToEmployer(commitmentId, accountId);

                await _mediator.SendAsync(new CreateApprenticeshipCommand
                {
                    AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                    Apprenticeship = await _apprenticeshipMapper.MapFrom(apprenticeship),
                    UserId = externalUserId,
                    UserEmailAddress = userEmail,
                    UserDisplayName = userName
                });
            }, apprenticeship.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeship(string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
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

                CheckCommitmentIsVisibleToEmployer(commitmentData.Commitment);

                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(apprenticeshipData.Apprenticeship, commitmentData.Commitment);

                apprenticeship.HashedAccountId = hashedAccountId;

                var overlaps = await _mediator.SendAsync(
                    new GetOverlappingApprenticeshipsQueryRequest
                    {
                        Apprenticeship = new[] { apprenticeshipData.Apprenticeship }
                    });

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(commitmentData.Commitment.TransferSender == null),
                        ValidationErrors = _apprenticeshipMapper.MapOverlappingErrors(overlaps)
                    }
                };
            }, hashedAccountId, externalUserId);
        }


        public async Task<OrchestratorResponse<ApprenticeshipViewModel>> GetApprenticeshipViewModel(string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
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

                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(apprenticeshipData.Apprenticeship, commitmentData.Commitment);

                apprenticeship.HashedAccountId = hashedAccountId;

                return new OrchestratorResponse<ApprenticeshipViewModel>
                {
                    Data = apprenticeship
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship, string externalUserId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            var commitmentId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            _logger.Info($"Updating Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                await CheckCommitmentIsVisibleToEmployer(commitmentId, accountId);

                await _mediator.SendAsync(new UpdateApprenticeshipCommand
                {
                    AccountId = accountId,
                    Apprenticeship = await _apprenticeshipMapper.MapFrom(apprenticeship),
                    UserId = externalUserId,
                    UserName = userName,
                    UserEmail = userEmail
                });
            }, apprenticeship.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<FinishEditingViewModel>> GetFinishEditingViewModel(string hashedAccountId, string externalUserId, string hashedCommitmentId)
        {
            try
            {
                var accountId = _hashingService.DecodeValue(hashedAccountId);
                var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
                _logger.Info($"Getting Finish Editing Model, Account: {accountId}, CommitmentId: {commitmentId}");

                return await CheckUserAuthorization(async () =>
                {
                    var response = await _mediator.SendAsync(new GetCommitmentQueryRequest
                    {
                        AccountId = accountId,
                        CommitmentId = commitmentId
                    });

                    CheckCommitmentIsVisibleToEmployer(response.Commitment);

                    var legalEntity =
                        await GetLegalEntityByCode(hashedAccountId, externalUserId, response.Commitment.LegalEntityId);

                    var hasSigned = HasSignedAgreement(legalEntity, response.Commitment.TransferSender!=null);

                    var overlaps = await _mediator.SendAsync(
                        new GetOverlappingApprenticeshipsQueryRequest
                        {
                            Apprenticeship = response.Commitment.Apprenticeships
                        });


                    return new OrchestratorResponse<FinishEditingViewModel>
                    {
                        Data = new FinishEditingViewModel
                        {
                            HashedAccountId = hashedAccountId,
                            HashedCommitmentId = hashedCommitmentId,
                            NotReadyForApproval = !response.Commitment.CanBeApproved,
                            ApprovalState = GetApprovalState(response.Commitment),
                            HasApprenticeships = response.Commitment.Apprenticeships.Any(),
                            InvalidApprenticeshipCount = response.Commitment.Apprenticeships.Count(x => !x.CanBeApproved),
                            HasSignedTheAgreement = hasSigned,
                            HasOverlappingErrors = overlaps?.Overlaps?.Any() ?? false
                        }
                    };
                }, hashedAccountId, externalUserId);
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<FinishEditingViewModel>
                {
                    FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(ex.ErrorMessages),
                    Status = HttpStatusCode.BadRequest
                };

            }
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse<FinishEditingViewModel>
                {
                    FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string>()),
                    Status = HttpStatusCode.Unauthorized
                };
            }

        }

        public async Task ApproveCommitment(string hashedAccountId, string externalUserId, string userDisplayName, string userEmail, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Approving Commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () =>
            {
                var lastAction = saveStatus == SaveStatus.AmendAndSend
                    ? LastAction.Amend
                    : LastAction.Approve;

                await _mediator.SendAsync(new SubmitCommitmentCommand
                {
                    EmployerAccountId = accountId,
                    CommitmentId = commitmentId,
                    HashedCommitmentId = hashedAccountId,
                    Message = string.Empty,
                    LastAction = lastAction,
                    UserDisplayName = userDisplayName,
                    UserEmailAddress = userEmail,
                    UserId = externalUserId
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task SetTransferApprovalStatus(string hashedAccountId, string hashedCommitmentId, TransferApprovalConfirmationViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var transferSenderId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Transfer Approval Confirmation: Sender Account: {transferSenderId}, CommitmentId: {commitmentId}, Approving {model.ApprovalConfirmed}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new TransferApprovalCommand
                {
                    CommitmentId = commitmentId,
                    TransferSenderId = transferSenderId,
                    TransferStatus = model.ApprovalConfirmed == true ? TransferApprovalStatus.Approved : TransferApprovalStatus.Rejected,
                    UserEmail = userEmail,
                    UserName = userDisplayName
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task SetTransferRequestApprovalStatus(string hashedAccountId, string hashedCommitmentId, string hashedTransferRequestId, TransferApprovalConfirmationViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var transferSenderId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            var transferRequestId = _hashingService.DecodeValue(hashedTransferRequestId);
            _logger.Info($"Transfer Approval Confirmation: Sender Account: {transferSenderId}, CommitmentId: {commitmentId}, Approving {model.ApprovalConfirmed}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new TransferApprovalCommand
                {
                    CommitmentId = commitmentId,
                    TransferSenderId = transferSenderId,
                    TransferRequestId = transferRequestId,
                    TransferStatus = model.ApprovalConfirmed == true ? TransferApprovalStatus.Approved : TransferApprovalStatus.Rejected,
                    UserEmail = userEmail,
                    UserName = userDisplayName
                });
            }, hashedAccountId, externalUserId);
        }


        public async Task<OrchestratorResponse<SubmitCommitmentViewModel>> GetSubmitNewCommitmentModel(string hashedAccountId, string externalUserId, string transferConnectionCode, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting Submit New Commitment ViewModel, Account: {accountId}");

            return await CheckUserAuthorization(() =>
            {
                return new OrchestratorResponse<SubmitCommitmentViewModel>
                {
                    Data = new SubmitCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        TransferConnectionCode = transferConnectionCode,
                        LegalEntityCode = legalEntityCode,
                        LegalEntityName = legalEntityName,
                        LegalEntityAddress = legalEntityAddress,
                        LegalEntitySource = legalEntitySource,
                        ProviderId = providerId,
                        ProviderName = providerName,
                        CohortRef = cohortRef,
                        SaveStatus = saveStatus
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SubmitCommitmentViewModel>> GetSubmitCommitmentModel(string hashedAccountId, string externalUserId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Submit Existing Commitment ViewModel, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = _hashingService.DecodeValue(hashedAccountId),
                    CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
                });

                CheckCommitmentIsVisibleToEmployer(data.Commitment);
                var commitment = _commitmentMapper.MapToCommitmentViewModel(data.Commitment);

                return new OrchestratorResponse<SubmitCommitmentViewModel>
                {
                    Data = new SubmitCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        ProviderName = commitment.ProviderName,
                        SaveStatus = saveStatus
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task SubmitCommitment(SubmitCommitmentViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            await CheckUserAuthorization(async () =>
            {
                if (model.SaveStatus != SaveStatus.Save)
                {
                    var accountId = _hashingService.DecodeValue(model.HashedAccountId);
                    var commitmentId = _hashingService.DecodeValue(model.HashedCommitmentId);
                    _logger.Info($"Submiting Commitment, Account: {accountId}, Commitment: {commitmentId}, Action: {model.SaveStatus}");

                    var lastAction = model.SaveStatus == SaveStatus.AmendAndSend
                        ? LastAction.Amend
                        : LastAction.Approve;

                    await _mediator.SendAsync(new SubmitCommitmentCommand
                    {
                        EmployerAccountId = _hashingService.DecodeValue(model.HashedAccountId),
                        CommitmentId = commitmentId,
                        HashedCommitmentId = model.HashedCommitmentId,
                        Message = model.Message,
                        LastAction = lastAction,
                        UserDisplayName = userDisplayName,
                        UserEmailAddress = userEmail,
                        UserId = externalUserId
                    });
                }
            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<AcknowledgementViewModel>> GetAcknowledgementModelForExistingCommitment(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Acknowldedgement Model for existing commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                return new OrchestratorResponse<AcknowledgementViewModel>
                {
                    Data = new AcknowledgementViewModel
                    {
                        HashedAccount = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        ProviderName = data.Commitment.ProviderName,
                        LegalEntityName = data.Commitment.LegalEntityName,
                        Message = GetLatestMessage(data.Commitment.Messages, false)?.Message,
                        IsTransfer = data.Commitment.TransferSender != null,
                        IsSecondApproval = data.Commitment.AgreementStatus == AgreementStatus.BothAgreed
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<YourCohortsViewModel>> GetYourCohorts(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitmentStatuses = (await GetAllCommitments(accountId)).Select(c => c.GetStatus()).ToArray();

                //todo: call into commitments api or db to get counts, seems excessive to fetch all cohorts data just to count

                return new OrchestratorResponse<YourCohortsViewModel>
                {
                    // The count of transfer funded cohorts in the bingo box doesn't actually
                    // refer to all transfer funded cohorts, but rather to just those
                    // transfer funded cohorts that are with the sender for approval
                    // or have been rejected by the sender.
                    // Transfer funded cohorts that are with the receiver or provider
                    // after having been rejected by the sender (and edited by the receiver)
                    // are counted as Draft cohorts instead.

                    Data = new YourCohortsViewModel
                    {
                        DraftCount = commitmentStatuses.Count(m =>
                             m == RequestStatus.NewRequest),
                        ReadyForReviewCount = commitmentStatuses.Count(m =>
                                m == RequestStatus.ReadyForReview
                             || m == RequestStatus.ReadyForApproval),
                        WithProviderCount = commitmentStatuses.Count(m =>
                             m == RequestStatus.WithProviderForApproval
                          || m == RequestStatus.SentToProvider
                          || m == RequestStatus.SentForReview),
                        TransferFundedCohortsCount = _featureToggleService.Get<Transfers>().FeatureEnabled
                            ? commitmentStatuses.Count(m => 
                                m == RequestStatus.WithSenderForApproval
                                || m == RequestStatus.RejectedBySender) : (int?)null
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllDraft(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts waiting to be sent for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
                {
                    var commitments = (await GetAllCommitmentsOfStatus(accountId,
                        RequestStatus.NewRequest)).ToArray();

                    return new OrchestratorResponse<CommitmentListViewModel>
                    {
                        Data = new CommitmentListViewModel
                        {
                            AccountHashId = hashedAccountId,
                            Commitments = MapFrom(commitments, true),
                            PageTitle = "Draft cohorts",
                            PageId = "draft-cohorts",
                            PageHeading = "Draft cohorts",
                            PageHeading2 = $"You have <strong>{commitments.Length}</strong> cohort{_addPluralizationSuffix(commitments.Length)} waiting to be sent to a training provider:",
                        }
                    };

                }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllReadyForReview(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts ready for review for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitments = (await GetAllCommitmentsOfStatus(accountId,
                    RequestStatus.ReadyForReview, RequestStatus.ReadyForApproval)).ToArray();

                return new OrchestratorResponse<CommitmentListViewModel>
                {
                    Data = new CommitmentListViewModel
                    {
                        AccountHashId = hashedAccountId,
                        Commitments = MapFrom(commitments, true),
                        PageTitle = "Cohorts for review",
                        PageId = "ready-for-review",
                        PageHeading = "Cohorts for review",
                        PageHeading2 = $"You have <strong>{commitments.Length}</strong> cohort{_addPluralizationSuffix(commitments.Length)} ready for review:"
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllWithProvider(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts with the provider sent for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitments = (await GetAllCommitmentsOfStatus(accountId, 
                    RequestStatus.WithProviderForApproval,
                    RequestStatus.SentForReview,
                    RequestStatus.SentToProvider)).ToArray();

                return new OrchestratorResponse<CommitmentListViewModel>
                {
                    Data = new CommitmentListViewModel
                    {
                        AccountHashId = hashedAccountId,
                        Commitments = MapFrom(commitments, false),
                        PageTitle = "Cohorts with training providers",
                        PageId = "with-the-provider",
                        PageHeading = "Cohorts with training providers",
                        PageHeading2 = $"You have <strong>{commitments.Length}</strong> cohort{_addPluralizationSuffix(commitments.Length)} with training providers for them to add apprentices, or review and approve details:"
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<TransferFundedCohortsViewModel>> GetAllTransferFunded(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your transfer-funded cohorts for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var transferFundedCommitments = await GetAllCommitmentsOfStatus(accountId,
                    RequestStatus.WithSenderForApproval, RequestStatus.RejectedBySender);

                return new OrchestratorResponse<TransferFundedCohortsViewModel>
                {
                    Data = new TransferFundedCohortsViewModel
                    {
                        Commitments = MapFrom(transferFundedCommitments)
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        private async Task<IEnumerable<CommitmentListItem>> GetAllCommitmentsOfStatus(long accountId, params RequestStatus[] statuses)
        {
            return (await GetAllCommitments(accountId)).Where(c => statuses.Contains(c.GetStatus()));
        }

        private async Task<IEnumerable<CommitmentListItem>> GetAllCommitments(long accountId)
        {
            _logger.Info($"Getting all Commitments for Account: {accountId}");

            var data = await _mediator.SendAsync(new GetCommitmentsQuery
            {
                AccountId = accountId
            });
            return data.Commitments;
        }

        public async Task<OrchestratorResponse<CommitmentDetailsViewModel>> GetCommitmentDetails(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment Details, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                CheckCommitmentIsVisibleToEmployer(data.Commitment);

                var overlappingApprenticeships = await _mediator.SendAsync(
                   new GetOverlappingApprenticeshipsQueryRequest
                   {
                       Apprenticeship = data.Commitment.Apprenticeships
                   });

                var apprenticships = data.Commitment.Apprenticeships?.Select(
                    a => MapToApprenticeshipListItem(a, overlappingApprenticeships)).ToList() ?? new List<ApprenticeshipListItemViewModel>(0);

                var trainingProgrammes = await GetTrainingProgrammes(data.Commitment.TransferSender == null);
                var apprenticeshipGroups = new List<ApprenticeshipListItemGroupViewModel>();

                var errors = new Dictionary<string, string>();
                var warnings = new Dictionary<string, string>();

                foreach (var group in apprenticships.OrderBy(x => x.TrainingName).GroupBy(x => x.TrainingCode))
                {
                    var apprenticeshipListGroup = new ApprenticeshipListItemGroupViewModel
                    {
                        Apprenticeships = group.OrderBy(x => x.CanBeApproved).ToList(),
                        TrainingProgramme = trainingProgrammes.FirstOrDefault(x => x.Id == group.Key)
                    };

                    apprenticeshipGroups.Add(apprenticeshipListGroup);

                    var trainingTitle = string.Empty;
                    if (!string.IsNullOrEmpty(apprenticeshipListGroup.TrainingProgramme?.Title))
                    {
                        trainingTitle = $":{apprenticeshipListGroup.TrainingProgramme.Title}";
                    }

                    if (apprenticeshipListGroup.OverlapErrorCount > 0)
                    {
                        errors.Add($"{apprenticeshipListGroup.GroupId}", $"Overlapping training dates{trainingTitle}");
                    }
                    
                    if (apprenticeshipListGroup.ApprenticeshipsOverFundingLimit > 0)
                    {
                        warnings.Add(apprenticeshipListGroup.GroupId, $"Cost for {apprenticeshipListGroup.TrainingProgramme.Title}");
                    }
                }

                var pageTitle = data.Commitment.EditStatus == EditStatus.EmployerOnly
                                || data.Commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Pending
                                    ? "Review your cohort"
                                    : "View your cohort";

                var viewModel = new CommitmentDetailsViewModel
                {
                    HashedId = _hashingService.HashValue(data.Commitment.Id),
                    Name = data.Commitment.Reference,
                    LegalEntityName = data.Commitment.LegalEntityName,
                    ProviderName = data.Commitment.ProviderName,
                    Status = data.Commitment.GetStatus(),
                    HasApprenticeships = apprenticships.Count > 0,
                    Apprenticeships = apprenticships,
                    ShowApproveOnlyOption = data.Commitment.AgreementStatus == AgreementStatus.ProviderAgreed,
                    LatestMessage = GetLatestMessage(data.Commitment.Messages, true)?.Message,
                    ApprenticeshipGroups = apprenticeshipGroups,
                    HasOverlappingErrors = apprenticeshipGroups.Any(m => m.ShowOverlapError),
                    IsReadOnly = data.Commitment.EditStatus != EditStatus.EmployerOnly,
                    Warnings = warnings,
                    Errors = errors,
                    PageTitle = pageTitle,
                    HideDeleteButton = data.Commitment.TransferSender?.Id != null
                };

                return new OrchestratorResponse<CommitmentDetailsViewModel>
                {
                    Data = viewModel
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<DeleteCommitmentViewModel>> GetDeleteCommitmentModel(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            return await CheckUserAuthorization(
                async () =>
                    {
                        var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
                        {
                            AccountId = accountId,
                            CommitmentId = commitmentId
                        });

                        CheckCommitmentIsVisibleToEmployer(commitmentData.Commitment);

                        Func<string, string> textOrDefault = txt => !string.IsNullOrEmpty(txt) ? txt : "without training course details";
                        var programmeSummary = commitmentData.Commitment.Apprenticeships
                                .GroupBy(m => m.TrainingName)
                                .Select(m => $"{m.Count()} {textOrDefault(m.Key)}")
                                .ToList();

                        return new OrchestratorResponse<DeleteCommitmentViewModel>
                        {
                            Data = new DeleteCommitmentViewModel
                            {
                                HashedAccountId = hashedAccountId,
                                HashedCommitmentId = hashedCommitmentId,
                                ProviderName = commitmentData.Commitment.ProviderName,
                                NumberOfApprenticeships = commitmentData.Commitment.Apprenticeships.Count,
                                ProgrammeSummaries = programmeSummary
                            }
                        };
                    }, hashedAccountId, externalUserId);
        }

        public async Task DeleteCommitment(string hashedAccountId, string hashedCommitmentId, string externalUserId, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            _logger.Info($"Deleting commitment {hashedCommitmentId} Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new DeleteCommitmentCommand
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId,
                    UserId = externalUserId,
                    UserDisplayName = userName,
                    UserEmailAddress = userEmail
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<DeleteApprenticeshipConfirmationViewModel>> GetDeleteApprenticeshipViewModel(string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            return await CheckUserAuthorization(async () =>
            {
                var apprenticeship = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                await CheckCommitmentIsVisibleToEmployer(commitmentId, accountId);

                return new OrchestratorResponse<DeleteApprenticeshipConfirmationViewModel>
                {
                    Data = new DeleteApprenticeshipConfirmationViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        HashedApprenticeshipId = hashedApprenticeshipId,
                        ApprenticeshipName = apprenticeship.Apprenticeship.ApprenticeshipName,
                        DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth.HasValue ? apprenticeship.Apprenticeship.DateOfBirth.Value.ToGdsFormat() : string.Empty
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<bool> AnyCohortsForCurrentStatus(string hashedAccountId, params RequestStatus[] requestStatusFromSession)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var allCommitments = await GetAllCommitments(accountId);
            return allCommitments.Any(c => requestStatusFromSession.Contains(c.GetStatus()));
        }

        public async Task<OrchestratorResponse<LegalEntitySignedAgreementViewModel>> GetLegalEntitySignedAgreementViewModel(string hashedAccountId, string transferConnectionCode, string legalEntityCode, string cohortRef, string userId)
        {
            var response = new OrchestratorResponse<LegalEntitySignedAgreementViewModel>();
            try
            {
                var legalEntity = await GetLegalEntityByCode(hashedAccountId, userId, legalEntityCode);

                var hasSigned = HasSignedAgreement(legalEntity, !string.IsNullOrWhiteSpace(transferConnectionCode));
                
                response.Data = new LegalEntitySignedAgreementViewModel
                {
                    HashedAccountId = hashedAccountId,
                    LegalEntityCode = legalEntityCode,
                    TransferConnectionCode = transferConnectionCode,
                    CohortRef = cohortRef,
                    HasSignedAgreement = hasSigned,
                    LegalEntityName = legalEntity.Name ?? string.Empty
                };

                return response;
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<LegalEntitySignedAgreementViewModel>
                {
                    Data = new LegalEntitySignedAgreementViewModel(),
                    Exception = ex,
                    FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(ex.ErrorMessages),
                    Status = HttpStatusCode.BadRequest
                };
            }
        }

        public async Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            var overlappingErrors = await _mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapFrom(apprenticeship) }
                });

            return _apprenticeshipMapper.MapOverlappingErrors(overlappingErrors);
        }

        public async Task DeleteApprenticeship(DeleteApprenticeshipConfirmationViewModel model, string externalUser, string userName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(model.HashedApprenticeshipId);

            await CheckUserAuthorization(async () =>
                    {
                        await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                        await _mediator.SendAsync(new DeleteApprenticeshipCommand
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId,
                            UserId = externalUser,
                            UserDisplayName = userName,
                            UserEmailAddress = userEmail
                        });

                    }, model.HashedAccountId, externalUser);
        }

        public async Task<OrchestratorResponse<TransferCommitmentViewModel>> GetCommitmentDetailsForTransfer(
            string hashedTransferAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedTransferAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment Details, Transfer Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId,
                    CallerType = CallerType.TransferSender
                });

                var viewModel = _commitmentMapper.MapToTransferCommitmentViewModel(data.Commitment);

                return new OrchestratorResponse<TransferCommitmentViewModel>
                {
                    Data = viewModel
                };
            }, hashedTransferAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<TransferRequestViewModel>> GetTransferRequestDetails(
            string hashedTransferAccountId, Application.Queries.GetTransferRequest.CallerType callerType, string hashedTransferRequestId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedTransferAccountId);
            var transferRequestId = _hashingService.DecodeValue(hashedTransferRequestId);
            _logger.Info($"Getting TransferRequest Details, Transfer Account: {accountId}, TransferRequestId: {transferRequestId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetTransferRequestQueryRequest
                {
                    AccountId = accountId,
                    TransferRequestId = transferRequestId,
                    CallerType = callerType
                });

                var viewModel = _commitmentMapper.MapToTransferRequestViewModel(data.TransferRequest);

                return new OrchestratorResponse<TransferRequestViewModel>
                {
                    Data = viewModel
                };
            }, hashedTransferAccountId, externalUserId);
        }

        private async Task<(long?, string)> GetTransferConnectionInfo(string hashedAccountId, string transferConnectionCode, string externalUserId)
        {
            string transferSenderName = null;
            long? transferSenderId = null;

            if (!string.IsNullOrEmpty(transferConnectionCode))
            {
                var data = await GetTransferConnectionsNoAuthorizationCheck(hashedAccountId, externalUserId);
                var transferConnections = _commitmentMapper.MapToTransferConnectionsViewModel(data.TransferConnections);

                var transferConnection = transferConnections.Single(x =>
                    x.TransferConnectionCode.Equals(transferConnectionCode, StringComparison.InvariantCultureIgnoreCase));
                transferSenderId = _publicHashingService.DecodeValue(transferConnectionCode);
                transferSenderName = transferConnection.TransferConnectionName;
            }

            return (transferSenderId, transferSenderName);
        }
        private static string CreateReference()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        private static ApprovalState GetApprovalState(CommitmentView commitment)
        {
            if (!commitment.Apprenticeships.Any()) return ApprovalState.ApproveAndSend;

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                || m.AgreementStatus == AgreementStatus.EmployerAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;

            return approvalState;
        }

        private async Task<GetAccountLegalEntitiesResponse> GetActiveLegalEntities(string hashedAccountId, string externalUserId)
        {
            return await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });
        }

        private async Task<LegalEntity> GetLegalEntityByCode(string hashedAccountId, string userId, string legalEntityCode)
        {
            var legalEntities = await GetActiveLegalEntities(hashedAccountId, userId);

            var legalEntity = legalEntities.LegalEntities.FirstOrDefault(
                    c => c.Code.Equals(legalEntityCode, StringComparison.CurrentCultureIgnoreCase));

            if (legalEntity == null)
            {
                throw new InvalidRequestException(new Dictionary<string, string> { { "LegalEntity", "Agreement does not exist" } });
            }
            return legalEntity;
        }

        private CommitmentViewModel MapFrom(CommitmentView commitment)
        {
            return new CommitmentViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName
            };
        }

        private IEnumerable<CommitmentListItemViewModel> MapFrom(IEnumerable<CommitmentListItem> commitments, bool showEmployer)
        {
            return commitments.Select(m => MapFrom(m, GetLatestMessage(m.Messages, showEmployer)?.Message));
        }

        private IEnumerable<TransferFundedCohortsListItemViewModel> MapFrom(IEnumerable<CommitmentListItem> commitments)
        {
            //todo: throw if TransferApprovalStatus == Approved?
            return commitments.Select(c => new TransferFundedCohortsListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(c.Id),
                SendingEmployer = c.TransferSenderName,
                ProviderName = c.ProviderName,
                TransferApprovalStatus = c.TransferApprovalStatus,
                ShowLink = c.TransferApprovalStatus == TransferApprovalStatus.Rejected ? ShowLink.Edit : ShowLink.Details,
            });
        }

        private MessageView GetLatestMessage(IEnumerable<MessageView> messages, bool showEmployer)
        {
            return messages.Where(x => x.CreatedBy == (showEmployer ? MessageCreator.Provider : MessageCreator.Employer)).OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem commitment, string latestMessage)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.GetStatus(),
                LatestMessage = latestMessage
            };
        }

        private ApprenticeshipListItemViewModel MapToApprenticeshipListItem(Apprenticeship apprenticeship, GetOverlappingApprenticeshipsQueryResponse overlappingApprenticeships)
        {
            return new ApprenticeshipListItemViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                ApprenticeName = apprenticeship.ApprenticeshipName,
                ApprenticeDateOfBirth = apprenticeship.DateOfBirth,
                ApprenticeUln = apprenticeship.ULN,
                TrainingCode = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                CanBeApproved = apprenticeship.CanBeApproved,
                OverlappingApprenticeships = overlappingApprenticeships.GetOverlappingApprenticeships(apprenticeship.Id)
            };
        }


        private async Task CheckCommitmentIsVisibleToEmployer(long commitmentId, long accountId)
        {
            var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId
            });
            CheckCommitmentIsVisibleToEmployer(commitmentData.Commitment);
        }

        private static void CheckCommitmentIsVisibleToEmployer(CommitmentView commitment)
        {
            //what are we trying to achieve here? we don't really want to assert the state of the commitment generally
            //we are trying to assert that the commitment is in the right state for the employer user to view it
            //(editability is something else)

            //a commitment can be viewed by an employer user if:
            // a) for non-transfer cohort, the agreement status is not BothAgreed
            // b) for a transfer cohort:
            //    i) the transfer approval status is not Approved (the agreement status can be anything)
            // this is the definition of "right of the line" for respective cases, thus not visible to user.

            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if(commitment.TransferSender != null && commitment.TransferSender.TransferApprovalStatus == TransferApprovalStatus.Approved)
                throw new InvalidStateException("Invalid commitment state - approved by transfer sender");

            if (commitment.TransferSender == null && commitment.AgreementStatus == AgreementStatus.BothAgreed)
                throw new InvalidStateException("Invalid commitment state - agreement status is BothAgreed");
        }

        private static bool HasSignedAgreement(LegalEntity legalEntity, bool isTransfer)
        {
            if (isTransfer)
            {
                return legalEntity.Agreements.Any(a =>
                    a.Status == EmployerAgreementStatus.Signed && a.TemplateVersionNumber == 2);
            }

            return legalEntity.Agreements.Any(a =>
                    a.Status == EmployerAgreementStatus.Signed &&
                    (a.TemplateVersionNumber == 1 || a.TemplateVersionNumber == 2));
        }
    }
}