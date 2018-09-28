using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitments;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCallingGetAllRejectedTransferFunded
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenItReturnsTheRejectedCommitmentsFromMediator(
            string hashedId,
            string externalUserId,
            string hashedCommitmentId,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IHashingService> mockHashingService,
            GetUserAccountRoleResponse getUserAccountRoleResponse,
            GetCommitmentsResponse getCommitmentsResponse,
            EmployerCommitmentsOrchestrator sut)
        {
            // this is needed to fulfill the CommitmentStatusCalculator which isn't DI'd.
            getCommitmentsResponse.Commitments.ForEach(delegate(CommitmentListItem item)
            {
                item.EditStatus = item.TransferApprovalStatus == TransferApprovalStatus.Rejected 
                    ? EditStatus.EmployerOnly 
                    : EditStatus.Both;
            });

            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(getUserAccountRoleResponse);
            mockMediator
                .Setup(mediator => mediator.SendAsync(It.IsAny<GetCommitmentsQuery>()))
                .ReturnsAsync(getCommitmentsResponse);

            mockHashingService
                .Setup(service => service.HashValue(It.IsAny<long>()))
                .Returns(hashedCommitmentId);

            var actualCommitments = 
                (await sut.GetAllRejectedTransferFunded(hashedId, externalUserId))
                .Data.Commitments.ToList();

            var expectedRejected = getCommitmentsResponse.Commitments.Where(item =>
                item.TransferApprovalStatus == TransferApprovalStatus.Rejected)
                .ToList();

            actualCommitments.Should().HaveCount(expectedRejected.Count);

            for (var i = 0; i < actualCommitments.Count; i++)
            {
                actualCommitments[i].TransferApprovalStatus.Should().Be(TransferApprovalStatus.Rejected);
                actualCommitments[i].ShowLink.Should().Be(ShowLink.Edit);
                actualCommitments[i].SendingEmployer.Should().Be(expectedRejected[i].TransferSenderName);
                actualCommitments[i].ProviderName.Should().Be(expectedRejected[i].ProviderName);
                actualCommitments[i].HashedCommitmentId.Should().Be(hashedCommitmentId);
            }
        }
    }
}