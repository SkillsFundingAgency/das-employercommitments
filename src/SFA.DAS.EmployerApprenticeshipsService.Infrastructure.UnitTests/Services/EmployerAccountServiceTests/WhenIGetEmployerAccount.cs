using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.EmployerAccountServiceTests
{
    public class WhenIGetEmployerAccount
    {
        private EmployerAccountService _employerAccountService;
        private Mock<IAccountApiClient> _accountApiClient;
        private const long AccountId = 12345;
        private AccountDetailViewModel _accountDetailViewModel;

        [SetUp]
        public void Arrange()
        {
            _accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = AccountId,
                ApprenticeshipEmployerType = "Levy"
            };

            _accountApiClient = new Mock<IAccountApiClient>();
            _accountApiClient.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(_accountDetailViewModel);

            _employerAccountService = new EmployerAccountService(_accountApiClient.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledWithTheCorrectParameters()
        {
            await _employerAccountService.GetAccount(AccountId);

            _accountApiClient.Verify(x=>x.GetAccount(AccountId));
        }

        [TestCase("Levy")]
        [TestCase("NonLevy")]
        public async Task ThenAccountDetailViewModelReturned(string apprenticeshipEmployerType)
        {
            _accountDetailViewModel.ApprenticeshipEmployerType = apprenticeshipEmployerType;

            var result = await _employerAccountService.GetAccount(AccountId);

            Assert.AreEqual(_accountDetailViewModel.AccountId, result.Id);
            Assert.AreEqual(_accountDetailViewModel.ApprenticeshipEmployerType, result.ApprenticeshipEmployerType.ToString());
        }
    }
}
