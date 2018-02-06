using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.EmployerAccountServiceTests
{
    public class WhenIGetTransferringEntitiesLinkedToAnAccount
    {
        private EmployerAccountService _employerAccountService;
        private Mock<IAccountApiClient> _accountApiClient;

        [SetUp]
        public void Arrange()
        {
            _accountApiClient = new Mock<IAccountApiClient>();
            _employerAccountService = new EmployerAccountService(_accountApiClient.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledAndGetsAnEmptyListOfTransferingEntitiesForAnAccount()
        {
            //Arrange
            var expectedAccountId = "ABC3421";
            _accountApiClient.Setup(x => x.GetTransferConnections(expectedAccountId))
                .ReturnsAsync(new List<TransferConnectionViewModel>());

            //Act
            var transferingEntities = await _employerAccountService.GetTransferConnectionsForAccount(expectedAccountId);

            //Assert
            _accountApiClient.Verify(x=>x.GetTransferConnections(expectedAccountId), Times.Once);
            Assert.IsEmpty(transferingEntities); 
        }

        [Test]
        public async Task ThenTheApiIsCalledAndGetsANonEmptyListOfTransferingEntitiesForAnAccount()
        {
            //Arrange
            _accountApiClient.Setup(x => x.GetTransferConnections(It.IsAny<string>()))
                .ReturnsAsync(new List<TransferConnectionViewModel>
                {
                    new TransferConnectionViewModel
                    {
                        TransferConnectionId = 1,
                        FundingEmployerAccountId = 1234,
                        FundingEmployerAccountName = "FirstAccountName",
                        FundingEmployerHashedAccountId = "FAN"
                    },
                    new TransferConnectionViewModel
                    {
                        TransferConnectionId = 2,
                        FundingEmployerAccountId = 1235,
                        FundingEmployerAccountName = "SecondAccountName",
                        FundingEmployerHashedAccountId = "SAN"
                    }

                });

            //Act
            var transferingEntities = await _employerAccountService.GetTransferConnectionsForAccount("ABCD");

            //Assert
            Assert.AreEqual(2, transferingEntities.Count);
            Assert.AreEqual(1234, transferingEntities[0].AccountId);
            Assert.AreEqual("FirstAccountName", transferingEntities[0].AccountName);
            Assert.AreEqual(1235, transferingEntities[1].AccountId);
            Assert.AreEqual("SecondAccountName", transferingEntities[1].AccountName);
        }

        [Test]
        public async Task ThenAnEmptyListIsReturnedIfTransferingConnectionsReturnsNull()
        {
            // Arrange
            _accountApiClient.Setup(x => x.GetTransferConnections(It.IsAny<string>()))
                .ReturnsAsync(new List<TransferConnectionViewModel>());

            //Act
            var actual = await _employerAccountService.GetTransferConnectionsForAccount("ABCD");

            //Assert
            Assert.IsEmpty(actual);
        }
    }
}
