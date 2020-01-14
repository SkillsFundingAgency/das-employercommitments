using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingApprenticeshipEmployerType : OrchestratorTestBase
    {
        const long AccountId = 1234;
        const string HashedAccountId = "ABC1234";
        private Account Account;
        
        [SetUp]
        public void Arrange()
        {
            Account = new Account { Id = AccountId, ApprenticeshipEmployerType = CommitmentsV2.Types.ApprenticeshipEmployerType.Levy};
            MockEmployerAccountService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(Account);
            MockHashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns((string param) => Convert.ToInt64(param.Remove(0, 3)));
        }

        [Test]
        public async Task ShouldCallServiceWithDecodedAccountId()
        {
            await EmployerCommitmentOrchestrator.GetApprenticeshipEmployerType(HashedAccountId);

            MockEmployerAccountService.Verify(x => x.GetAccount(AccountId), Times.Once);
        }

        [Test]
        public async Task ShouldReturnAccount()
        {
            var result = await EmployerCommitmentOrchestrator.GetApprenticeshipEmployerType(HashedAccountId);
            Assert.AreEqual(Account.ApprenticeshipEmployerType, result);
        }
    }
}
