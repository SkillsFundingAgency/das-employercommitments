using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public class CommitmentsBaseOrchestrator
    {
        protected readonly IMediator Mediator;
        protected readonly IHashingService HashingService;
        protected readonly ILog Logger;

        public CommitmentsBaseOrchestrator(
            IMediator mediator, 
            IHashingService hashingService,
            ILog logger)
        {
            Mediator = mediator;
            HashingService = hashingService;
            Logger = logger;
        }

        public async Task<bool> AuthorizeRole(string hashedAccountId, string externalUserId, Role[] roles)
        {
            var response = await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });

            return response.User != null && roles.Contains(response.User.Role);
        }

        protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, string hashedAccountId, string externalUserId) where T : class
        {
            try
            {
                await CheckUserIsConnectedToAccount(hashedAccountId, externalUserId);

                return await code.Invoke();
            }
            catch (UnauthorizedAccessException exception)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);

                return new OrchestratorResponse<T>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = exception
                };
            }
        }

        protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<OrchestratorResponse<T>> code, string hashedAccountId, string externalUserId) where T : class
        {
            try
            {
                await CheckUserIsConnectedToAccount(hashedAccountId, externalUserId);

                return code.Invoke();
            }
            catch (UnauthorizedAccessException exception)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);

                return new OrchestratorResponse<T>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = exception
                };
            }
        }

        protected async Task CheckUserAuthorization(Func<Task> code, string hashedAccountId, string externalUserId)
        {
            try
            {
                await CheckUserIsConnectedToAccount(hashedAccountId, externalUserId);

                await code.Invoke();
            }
            catch (UnauthorizedAccessException)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);
            }
        }

        protected async Task<List<TrainingProgramme>> GetTrainingProgrammes(bool includeFrameworks)
        {
            var programmes = await Mediator.SendAsync(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = includeFrameworks
            });
            return programmes.TrainingProgrammes;
        }

        private async Task CheckUserIsConnectedToAccount(string hashedAccountId, string externalUserId)
        {
            var response = await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });

            if (response.User == null)
            {
                throw new UnauthorizedAccessException();
            }
        }

        private void LogUnauthorizedUserAttempt(string hashedAccountId, string externalUserId)
        {
            var accountId = HashingService.DecodeValue(hashedAccountId);
            Logger.Warn($"User not associated to account. UserId:{externalUserId} AccountId:{accountId}");
        }
    }
}