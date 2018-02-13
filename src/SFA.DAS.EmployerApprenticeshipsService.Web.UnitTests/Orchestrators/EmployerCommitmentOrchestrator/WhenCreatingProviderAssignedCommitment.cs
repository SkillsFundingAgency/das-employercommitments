using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCreatingProviderAssignedCommitment : OrchestratorTestBase
    {
        private SubmitCommitmentViewModel _viewModel;
        private CreateCommitmentCommandResponse _sendAsyncResponse;
        private GetAccountTransferConnectionsResponse _getTransferConnectionsResponse;
        const long UnhashedAccountId = 123;
        private const string TransferConnectionCode = "TC999";
        private const long TransferSenderId = 999;
        private const string TransferConnectionName = "TCName";

        [SetUp]
        public void Arrange()
        {
            _sendAsyncResponse = new CreateCommitmentCommandResponse { CommitmentId = 123 };
            _getTransferConnectionsResponse = new GetAccountTransferConnectionsResponse
            {
                TransferConnections = new List<TransferConnection> { new TransferConnection { AccountName = TransferConnectionName, HashedAccountId = TransferConnectionCode } }
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<CreateCommitmentCommand>())).ReturnsAsync(_sendAsyncResponse);
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransferConnectionsRequest>())).ReturnsAsync(_getTransferConnectionsResponse);

            _viewModel = new SubmitCommitmentViewModel
            {
                Message = "Message",
                CohortRef = "CohortRef",
                HashedAccountId = "HashedAccountId",
                LegalEntityCode = "LegalEntityCode",
                LegalEntityName = "LegalEntityName",
                LegalEntityAddress = "LegalEntityAddress",
                LegalEntitySource = (short) OrganisationType.Other,
                ProviderId = "1",
                ProviderName = "ProviderName",
                TransferConnectionCode = null
            };

            MockHashingService.Setup(x => x.DecodeValue(_viewModel.HashedAccountId)).Returns(UnhashedAccountId);
            MockHashingService.Setup(x => x.DecodeValue(TransferConnectionCode)).Returns(TransferSenderId);
        }

        [Test]
        public async Task ShouldMapPropertiesToCommandWithNullTransferConnection()
        {
            //Act
            await EmployerCommitmentOrchestrator.CreateProviderAssignedCommitment(_viewModel, "externalUser",
                "DisplayName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(
                    It.Is<CreateCommitmentCommand>(p => p.UserId == "externalUser" &&
                                                        p.Message == _viewModel.Message &&
                                                        p.Commitment.Reference == _viewModel.CohortRef &&
                                                        p.Commitment.EmployerAccountId == UnhashedAccountId &&
                                                        p.Commitment.TransferSenderName == null &&
                                                        p.Commitment.TransferSenderId == null &&
                                                        p.Commitment.LegalEntityId == _viewModel.LegalEntityCode &&
                                                        p.Commitment.LegalEntityName == _viewModel.LegalEntityName &&
                                                        p.Commitment.LegalEntityAddress ==
                                                        _viewModel.LegalEntityAddress &&
                                                        p.Commitment.LegalEntityOrganisationType ==
                                                        (OrganisationType)_viewModel.LegalEntitySource &&
                                                        p.Commitment.ProviderId == 1 &&
                                                        p.Commitment.CommitmentStatus == CommitmentStatus.Active &&
                                                        p.Commitment.EditStatus == EditStatus.ProviderOnly &&
                                                        p.Commitment.EmployerLastUpdateInfo.Name == "DisplayName" &&
                                                        p.Commitment.EmployerLastUpdateInfo.EmailAddress ==
                                                        "UserEmail")),
                Times.Once);
            MockHashingService.Verify(x=>x.DecodeValue(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ShouldMapPropertiesToCommandWithATransferConnection()
        {
            //Arrange
            _viewModel.TransferConnectionCode = TransferConnectionCode;

            //Act
            await EmployerCommitmentOrchestrator.CreateProviderAssignedCommitment(_viewModel, "externalUser",
                "DisplayName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(
                    It.Is<CreateCommitmentCommand>(p => p.Commitment.TransferSenderName == TransferConnectionName && p.Commitment.TransferSenderId == TransferSenderId)),
                Times.Once);
        }
    }
}
