using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.ResolveRequestedChanges;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.EmployerCommitments.Application.Queries.GetPriceHistoryQueryRequest;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Exceptions;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class DataLockOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        private readonly IApprenticeshipMapper _apprenticeshipMapper;

        public DataLockOrchestrator(
            IMediator mediator, 
            IHashingService hashingService, 
            ILog logger,
            IApprenticeshipMapper apprenticeshipMapper
            ) : base(mediator, hashingService, logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _apprenticeshipMapper = apprenticeshipMapper;
        }

        public async Task<OrchestratorResponse<DataLockStatusViewModel>> GetDataLockStatusForRestartRequest(string hashedAccountId, string hashedApprenticeshipId, string userId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            return await CheckUserAuthorization(
                async () =>
                {

                    var dataLockSummary = await _mediator.SendAsync(
                        new GetDataLockSummaryQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    //var dataLock = dataLocks.DataLockStatus
                    //    .First(m => m.TriageStatus == TriageStatus.Restart);
                    var dataLock = dataLockSummary.DataLockSummary
                    .DataLockWithCourseMismatch.FirstOrDefault(m => m.TriageStatus == TriageStatus.Restart);

                    if (dataLock == null)
                        throw new InvalidStateException($"No data locks exist that can be restarted for apprenticeship: {apprenticeshipId}");

                    var apprenticeship = await _mediator.SendAsync(
                        new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    var programms = await GetTrainingProgrammes();
                    var currentProgram = programms.Single(m => m.Id == apprenticeship.Apprenticeship.TrainingCode);
                    var newProgram = programms.Single(m => m.Id == dataLock.IlrTrainingCourseCode);

                    return new OrchestratorResponse<DataLockStatusViewModel>
                    {
                        Data = new DataLockStatusViewModel
                        {
                            HashedAccountId = hashedAccountId,
                            HashedApprenticeshipId = hashedApprenticeshipId,
                            CurrentProgram = currentProgram,
                            IlrProgram = newProgram,
                            PeriodStartData = dataLock.IlrEffectiveFromDate,
                            ProviderName = apprenticeship.Apprenticeship.ProviderName,
                            LearnerName = apprenticeship.Apprenticeship.ApprenticeshipName,
                            DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth
                        }
                    };
                }, hashedAccountId, userId);
        }

        public async Task<OrchestratorResponse<DataLockStatusViewModel>> GetDataLockChangeStatus(string hashedAccountId, string hashedApprenticeshipId, string userId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            return await CheckUserAuthorization(
                async () =>
                {
                    var dataLockSummary = await _mediator.SendAsync(
                            new GetDataLockSummaryQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    if (!dataLockSummary.DataLockSummary.DataLockWithOnlyPriceMismatch.Any())
                        throw new InvalidStateException($"Apprenticeship does not contain any price data locks. Apprenticeship: {apprenticeshipId}");

                    var priceHistory = await _mediator.SendAsync(new GetPriceHistoryQueryRequest
                    {
                        AccountId = accountId,
                        ApprenticeshipId = apprenticeshipId
                    });

                    var apprenticeship = await _mediator.SendAsync(
                        new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    return new OrchestratorResponse<DataLockStatusViewModel>
                    {
                        Data = new DataLockStatusViewModel
                        {
                            HashedAccountId = hashedAccountId,
                            HashedApprenticeshipId = hashedApprenticeshipId,
                            PeriodStartData = new DateTime(2017, 08, 08),
                            ProviderName = apprenticeship.Apprenticeship.ProviderName,
                            LearnerName = apprenticeship.Apprenticeship.ApprenticeshipName,
                            DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth,
                            CourseChanges =  await _apprenticeshipMapper.MapCourseChanges(dataLockSummary.DataLockSummary.DataLockWithCourseMismatch, apprenticeship.Apprenticeship),
                            PriceChanges = _apprenticeshipMapper.MapPriceChanges(dataLockSummary.DataLockSummary.DataLockWithOnlyPriceMismatch, priceHistory.History)

                        }
                    };
                }, hashedAccountId, userId);
        }

        public async Task ConfirmRequestChanges(string hashedAccountId, string hashedApprenticeshipId, string user, bool approved)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await CheckUserAuthorization(
                async () =>
                {
                    await _mediator.SendAsync(
                        new ResolveRequestedChangesCommand
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId,
                            Approved = approved,
                            TriageStatus = TriageStatus.Change,
                            UserId = user
                        });
                },
                hashedAccountId,
                user);
        }
    }
}