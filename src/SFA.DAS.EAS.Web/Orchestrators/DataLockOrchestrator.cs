using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.ResolveRequestedChanges;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.EmployerCommitments.Application.Queries.GetPriceHistoryQueryRequest;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class DataLockOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ILinkGenerator _linkGenerator;

        public DataLockOrchestrator(
            IMediator mediator, 
            IHashingService hashingService, 
            ILog logger,
            IApprenticeshipMapper apprenticeshipMapper,
            ILinkGenerator linkGenerator
            ) : base(mediator, hashingService, logger)
        {
            _apprenticeshipMapper = apprenticeshipMapper;
            _linkGenerator = linkGenerator;
        }

        public async Task<OrchestratorResponse<DataLockStatusViewModel>> GetDataLockStatusForRestartRequest(string hashedAccountId, string hashedApprenticeshipId, string userId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            return await CheckUserAuthorization(
                async () =>
                {

                    var dataLockSummary = await Mediator.SendAsync(
                        new GetDataLockSummaryQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    //var dataLock = dataLocks.DataLockStatus
                    //    .First(m => m.TriageStatus == TriageStatus.Restart);
                    var dataLock = dataLockSummary.DataLockSummary
                    .DataLockWithCourseMismatch.FirstOrDefault(m => m.TriageStatus == TriageStatus.Restart);

                    if (dataLock == null)
                        throw new InvalidStateException($"No data locks exist that can be restarted for apprenticeship: {apprenticeshipId}");

                    var apprenticeship = await Mediator.SendAsync(
                        new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    var programms = await GetTrainingProgrammes(true);
                    var currentProgram = programms.Single(m => m.CourseCode == apprenticeship.Apprenticeship.TrainingCode);
                    var newProgram = programms.Single(m => m.CourseCode == dataLock.IlrTrainingCourseCode);

                    return new OrchestratorResponse<DataLockStatusViewModel>
                    {
                        Data = new DataLockStatusViewModel
                        {
                            HashedAccountId = hashedAccountId,
                            HashedApprenticeshipId = hashedApprenticeshipId,
                            CurrentProgram = currentProgram,
                            IlrProgram = newProgram,
                            PeriodStartData = dataLock.IlrEffectiveFromDate,
                            PeriodEndData = dataLock.IlrEffectiveToDate,
                            ProviderName = apprenticeship.Apprenticeship.ProviderName,
                            LearnerName = apprenticeship.Apprenticeship.ApprenticeshipName,
                            DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth,
                            ApprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details")
                        }
                    };
                }, hashedAccountId, userId);
        }

        public async Task<OrchestratorResponse<DataLockStatusViewModel>> GetDataLockChangeStatus(string hashedAccountId, string hashedApprenticeshipId, string userId)
        {   
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            return await CheckUserAuthorization(
                async () =>
                {
                    var dataLockSummary = await Mediator.SendAsync(
                            new GetDataLockSummaryQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    var priceHistory = await Mediator.SendAsync(new GetPriceHistoryQueryRequest
                    {
                        AccountId = accountId,
                        ApprenticeshipId = apprenticeshipId
                    });

                    var apprenticeship = await Mediator.SendAsync(
                        new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    var dataLockPrice =
                        dataLockSummary.DataLockSummary.DataLockWithCourseMismatch
                        .Concat(dataLockSummary.DataLockSummary.DataLockWithOnlyPriceMismatch)
                        .Where(m => m.ErrorCode.HasFlag(DataLockErrorCode.Dlock07));

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
                            CourseChanges =  await _apprenticeshipMapper.MapCourseChanges(dataLockSummary.DataLockSummary.DataLockWithCourseMismatch, apprenticeship.Apprenticeship, priceHistory.History),
                            PriceChanges = _apprenticeshipMapper.MapPriceChanges(dataLockPrice, priceHistory.History),
                            ULN = apprenticeship.Apprenticeship.ULN,
                            TrainingName = apprenticeship.Apprenticeship.TrainingName,
                            ApprenticeDetailsV2Link = _linkGenerator.CommitmentsV2Link($"{hashedAccountId}/apprentices/{hashedApprenticeshipId}/details")
                        }
                    };
                }, hashedAccountId, userId);
        }

        public async Task ConfirmRequestChanges(string hashedAccountId, string hashedApprenticeshipId, string user, bool approved)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            await CheckUserAuthorization(
                async () =>
                {
                    await Mediator.SendAsync(
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