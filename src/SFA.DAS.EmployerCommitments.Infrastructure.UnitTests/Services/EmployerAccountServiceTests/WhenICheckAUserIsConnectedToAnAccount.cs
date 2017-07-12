using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.EmployerAccountServiceTests
{
    public class WhenICheckAUserIsConnectedToAnAccount
    {
        private EmployerAccountService _employerAccountService;
        private Mock<IAccountApiClient> _accountApiClient;
        private const string ExpectedAccountId = "123RFV";
        private const string ExpectedUserId = "123RFV";

        [SetUp]
        public void Arrange()
        {
            _accountApiClient = new Mock<IAccountApiClient>();
            _accountApiClient.Setup(x => x.GetAccountUsers(ExpectedAccountId))
                .ReturnsAsync(new List<TeamMemberViewModel>
                {
                    new TeamMemberViewModel
                    {
                        UserRef = ExpectedUserId,
                        Role = "Owner"
                    }
                });

            _employerAccountService = new EmployerAccountService(_accountApiClient.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledWithTheCorrectParameters()
        {
            //Act
            await _employerAccountService.GetUserRoleOnAccount(ExpectedAccountId, ExpectedUserId);

            //Assert
            _accountApiClient.Verify(x=>x.GetAccountUsers(ExpectedAccountId));
        }

        [Test]
        public async Task ThenTheUsersRoleInTheAccountIsReturned()
        {
            //Act
            var actual = await _employerAccountService.GetUserRoleOnAccount(ExpectedAccountId, ExpectedUserId);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenIfTheUserIsNotConnectedToTheAccountThenNullIsReturned()
        {
            //Act
            var actual = await _employerAccountService.GetUserRoleOnAccount(ExpectedAccountId, "123EDC");

            //Assert
            Assert.IsNull(actual);
        }
    }
}
