using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.EmployerAccountServiceTests
{
    public class WhenIGetLegalEntitiesLinkedToAnAccount
    {
        private EmployerAccountService _employerAccountService;
        private Mock<IAccountApiClient> _accountApiClient;
        private const string ExpectedAccountId = "ABC3421";

        [SetUp]
        public void Arrange()
        {
            _accountApiClient = new Mock<IAccountApiClient>();
            _accountApiClient.Setup(x => x.GetLegalEntitiesConnectedToAccount(ExpectedAccountId))
                .ReturnsAsync(new List<ResourceViewModel>());

            _employerAccountService = new EmployerAccountService(_accountApiClient.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToGetTheListOfLegalEntitiesForAnAccount()
        {
            //Arrange
            var expectedAccountId = "ABC3421";

            //Act
            await _employerAccountService.GetLegalEntitiesForAccount(expectedAccountId);

            //Assert
            _accountApiClient.Verify(x=>x.GetLegalEntitiesConnectedToAccount(expectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenTheApiIsCalledForEachLegalEntityId()
        {
            //Arrange
            _accountApiClient.Setup(x => x.GetLegalEntitiesConnectedToAccount(ExpectedAccountId))
                .ReturnsAsync(new List<ResourceViewModel>
                {
                    new ResourceViewModel {Id = "4587"},
                    new ResourceViewModel {Id = "85214"}
                });
            _accountApiClient.Setup(x => x.GetLegalEntity(ExpectedAccountId, It.IsAny<long>()))
                .ReturnsAsync(new LegalEntityViewModel
                {
                    AgreementStatus = EmployerAgreementStatus.Pending,
                });

            //Act
            var actual = await _employerAccountService.GetLegalEntitiesForAccount(ExpectedAccountId);

            //Assert
            _accountApiClient.Verify(x=>x.GetLegalEntity(ExpectedAccountId, It.IsAny<long>()), Times.Exactly(2));
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task ThenNullIsReturnedIfNoLegalEntitiesExistForTheAccount()
        {
            //Act
            var actual = await _employerAccountService.GetLegalEntitiesForAccount(ExpectedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }
    }
}
