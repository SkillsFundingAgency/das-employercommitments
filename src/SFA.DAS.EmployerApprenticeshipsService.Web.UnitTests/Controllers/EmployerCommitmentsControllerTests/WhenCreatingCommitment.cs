using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerCommitmentsControllerTests
{
    [TestFixture]
    public class WhenCreatingCommitment : EmployerCommitmentsControllerTest
    {
        //split test into 2?
        [Test]
        public async Task AndSelectedRouteIsProviderThenRedirectToSubmitNewCommitmentWithCorrectRouteValues()
        {
            // autofixture?
            var createCommitmentViewModel = new CreateCommitmentViewModel
            {
                SelectedRoute = "Provider",

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

            var result = await Controller.CreateCommitment(TestHelper.Clone(createCommitmentViewModel));

            AssertRedirectAction(result, "SubmitNewCommitment", expectedRouteValues: new
            {
                hashedAccountId = createCommitmentViewModel.HashedAccountId,
                transferConnectionCode = createCommitmentViewModel.TransferConnectionCode,
                legalEntityCode = createCommitmentViewModel.LegalEntityCode,
                legalEntityName = createCommitmentViewModel.LegalEntityName,
                legalEntityAddress = createCommitmentViewModel.LegalEntityAddress,
                legalEntitySource = createCommitmentViewModel.LegalEntitySource,
                accountLegalEntityPublicHashedId = createCommitmentViewModel.AccountLegalEntityPublicHashedId,
                providerId = createCommitmentViewModel.ProviderId,
                providerName = createCommitmentViewModel.ProviderName,
                cohortRef = createCommitmentViewModel.CohortRef,
                saveStatus = SaveStatus.Save
            });
        }
    }
}
