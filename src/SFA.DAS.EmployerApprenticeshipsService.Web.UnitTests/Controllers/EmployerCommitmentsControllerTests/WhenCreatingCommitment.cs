using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerCommitmentsControllerTests
{
    [TestFixture]
    public class WhenCreatingCommitment : EmployerCommitmentsControllerTest
    {
        private CreateCommitmentViewModel _createCommitmentViewModel;

        [SetUp]
        public void WhenCreatingCommitmentSetup()
        {
            _createCommitmentViewModel = new CreateCommitmentViewModel
            {
                HashedAccountId = "HASHBROWN",
                TransferConnectionCode = "TRANNY",
                LegalEntityCode = "LEGCODE",
                LegalEntityName = "LEGNAME",
                LegalEntityAddress = "LEGADD",
                LegalEntitySource = 0,
                AccountLegalEntityPublicHashedId = "123456",
                ProviderId = 1,
                ProviderName = "HIREAPRO",
                CohortRef = "COREF"
            };
        }

        //split test into 2?
        [Test]
        public async Task AndSelectedRouteIsProviderThenRedirectToSubmitNewCommitmentWithCorrectRouteValues()
        {
            _createCommitmentViewModel.SelectedRoute = "provider";

            var result = await Controller.CreateCommitment(TestHelper.Clone(_createCommitmentViewModel));

            AssertRedirectAction(result, "SubmitNewCommitment", expectedRouteValues: new
            {
                hashedAccountId = _createCommitmentViewModel.HashedAccountId,
                transferConnectionCode = _createCommitmentViewModel.TransferConnectionCode,
                legalEntityCode = _createCommitmentViewModel.LegalEntityCode,
                legalEntityName = _createCommitmentViewModel.LegalEntityName,
                legalEntityAddress = _createCommitmentViewModel.LegalEntityAddress,
                legalEntitySource = _createCommitmentViewModel.LegalEntitySource,
                accountLegalEntityPublicHashedId = _createCommitmentViewModel.AccountLegalEntityPublicHashedId,
                providerId = _createCommitmentViewModel.ProviderId,
                providerName = _createCommitmentViewModel.ProviderName,
                cohortRef = _createCommitmentViewModel.CohortRef,
                saveStatus = SaveStatus.Save
            });
        }

        [Test]
        public async Task AndSelectedRouteIsEmployerThenRedirectToDetails()
        {
            const string hashedCommitmentId = "HashedCommitmentId";

            _createCommitmentViewModel.SelectedRoute = "employer";

            Orchestrator.Setup(o => o.CreateEmployerAssignedCommitment(It.IsAny<CreateCommitmentViewModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<string> {Data = hashedCommitmentId});

            var result = await Controller.CreateCommitment(TestHelper.Clone(_createCommitmentViewModel));

            AssertRedirectAction(result, "Details", expectedRouteValues: new
            {
                hashedCommitmentId
            });
        }

        [Test]
        public async Task AndSelectedRouteIsEmployerThenOrchestratorIsCalledToCreateCommitment()
        {
            const string hashedCommitmentId = "HashedCommitmentId";

            _createCommitmentViewModel.SelectedRoute = "employer";

            Orchestrator.Setup(o => o.CreateEmployerAssignedCommitment(It.IsAny<CreateCommitmentViewModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<string> { Data = hashedCommitmentId });

            await Controller.CreateCommitment(TestHelper.Clone(_createCommitmentViewModel));

            Orchestrator.Verify(o => o.CreateEmployerAssignedCommitment(
                It.Is<CreateCommitmentViewModel>(vm => AreEqual(_createCommitmentViewModel, vm)),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        private bool AreEqual(object obj1, object obj2)
        {
            var compare = new CompareLogic();
            var result = compare.Compare(obj1, obj2);
            return result.AreEqual;
        }
    }
}
