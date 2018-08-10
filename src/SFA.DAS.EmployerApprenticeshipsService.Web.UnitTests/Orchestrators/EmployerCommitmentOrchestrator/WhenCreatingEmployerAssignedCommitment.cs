using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCreatingEmployerAssignedCommitment : OrchestratorTestBase
    {
        private CreateCommitmentViewModel _viewModel;
        private CreateCommitmentCommandResponse _sendAsyncResponse;
        private GetAccountTransferConnectionsResponse _getTransferConnectionsResponse;
        private List<TransferConnectionViewModel> _transferConnections;

        const long UnhashedAccountId = 123;
        private const string TransferConnectionCode = "TC999";
        private const long TransferConnectionId = 999;
        private const long TransferSenderId = 999;
        private const string TransferConnectionName = "TCName";
        private const string AccountLegalEntityPublicHashedId = "123456";

        [SetUp]
        public void Arrange()
        {
            _sendAsyncResponse = new CreateCommitmentCommandResponse { CommitmentId = 123 };
            _getTransferConnectionsResponse = new GetAccountTransferConnectionsResponse
            {
                TransferConnections = new List<TransferConnection> { new TransferConnection { AccountName = TransferConnectionName, AccountId = TransferConnectionId } }
            };

            _transferConnections = new List<TransferConnectionViewModel> { new TransferConnectionViewModel { TransferConnectionCode = "TC999", TransferConnectionName = "TCName" } };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<CreateCommitmentCommand>())).ReturnsAsync(_sendAsyncResponse);
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransferConnectionsRequest>())).ReturnsAsync(_getTransferConnectionsResponse);

            _viewModel = new CreateCommitmentViewModel
            {
                CohortRef = "CohortRef",
                HashedAccountId = "HashedAccountId",
                LegalEntityCode = "LegalEntityCode",
                LegalEntityName = "LegalEntityName",
                LegalEntityAddress = "LegalEntityAddress",
                LegalEntitySource = (short) OrganisationType.Other,
                AccountLegalEntityPublicHashedId = AccountLegalEntityPublicHashedId,
                ProviderId = 2,
                ProviderName = "ProviderName",
                TransferConnectionCode = TransferConnectionCode
            };

            MockCommitmentMapper.Setup(x => x.MapToTransferConnectionsViewModel(It.IsAny<List<TransferConnection>>()))
                .Returns(_transferConnections);

            MockHashingService.Setup(x => x.DecodeValue(_viewModel.HashedAccountId)).Returns(UnhashedAccountId);
            MockPublicHashingService.Setup(x => x.DecodeValue(TransferConnectionCode)).Returns(TransferSenderId);
        }

        [Test]
        public async Task ShouldMapPropertiesToCommandWithTransferConnection()
        {
            //Act
            await EmployerCommitmentOrchestrator.CreateEmployerAssignedCommitment(_viewModel, "externalUser",
                "DisplayName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(
                    It.Is<CreateCommitmentCommand>(p => p.UserId == "externalUser" &&
                                                        p.Commitment.Reference == _viewModel.CohortRef &&
                                                        p.Commitment.EmployerAccountId == UnhashedAccountId &&
                                                        p.Commitment.TransferSenderName == TransferConnectionName &&
                                                        p.Commitment.TransferSenderId == TransferSenderId &&
                                                        p.Commitment.LegalEntityId == _viewModel.LegalEntityCode &&
                                                        p.Commitment.LegalEntityName == _viewModel.LegalEntityName &&
                                                        p.Commitment.LegalEntityAddress ==
                                                        _viewModel.LegalEntityAddress &&
                                                        p.Commitment.LegalEntityOrganisationType ==
                                                        (OrganisationType)_viewModel.LegalEntitySource &&
                                                        p.Commitment.AccountLegalEntityPublicHashedId ==
                                                        AccountLegalEntityPublicHashedId &&
                                                        p.Commitment.ProviderId == 2 &&
                                                        p.Commitment.CommitmentStatus == CommitmentStatus.New &&
                                                        p.Commitment.EditStatus == EditStatus.EmployerOnly &&
                                                        p.Commitment.EmployerLastUpdateInfo.Name == "DisplayName" &&
                                                        p.Commitment.EmployerLastUpdateInfo.EmailAddress == "UserEmail")),
                Times.Once);
        }

        [Test]
        public async Task ShouldCallTransferMappingFunction()
        {
            //Act
            await EmployerCommitmentOrchestrator.CreateEmployerAssignedCommitment(_viewModel, "externalUser",
                "DisplayName", "UserEmail");

            //Assert
            MockCommitmentMapper.Verify(x => x.MapToTransferConnectionsViewModel(_getTransferConnectionsResponse.TransferConnections));
        }

        [Test]
        public async Task ShouldMapPropertiesToCommandWithoutTransferConnection()
        {
            //Arrange
            _viewModel.TransferConnectionCode = null;

            //Act
            await EmployerCommitmentOrchestrator.CreateEmployerAssignedCommitment(_viewModel, "externalUser",
                "DisplayName", "UserEmail");

            //Assert
            MockMediator.Verify(x => x.SendAsync(
                    It.Is<CreateCommitmentCommand>(p => p.UserId == "externalUser" &&
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
                                                        p.Commitment.AccountLegalEntityPublicHashedId ==
                                                        AccountLegalEntityPublicHashedId &&
                                                        p.Commitment.ProviderId == 2 &&
                                                        p.Commitment.CommitmentStatus == CommitmentStatus.New &&
                                                        p.Commitment.EditStatus == EditStatus.EmployerOnly &&
                                                        p.Commitment.EmployerLastUpdateInfo.Name == "DisplayName" &&
                                                        p.Commitment.EmployerLastUpdateInfo.EmailAddress ==
                                                        "UserEmail")),
                Times.Once);
        }
    }
}
