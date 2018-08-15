using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Enums;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCallingGetSubmitNewCommitmentModel : OrchestratorTestBase
    {
        private const string HashedAccountId = "HASH";

        [Test]
        public async Task ShouldReturnCorrectSubmitCommitmentViewModel()
        {
            const string externalUserId = "Bob";
            const string transferConnectionCode = "TCC";
            const string legalEntityCode = "LECODE";
            const string legalEntityName = "LENAME";
            const string legalEntityAddress = "LEADD";
            const short legalEntitySource = 0;
            const string accountLegalEntityPublicHashedId = "123456";
            const string providerId = "123";
            const string providerName = "PRONAME";
            const string cohortRef = "COREF";
            const SaveStatus saveStatus = Enums.SaveStatus.Approve;

            MockHashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(1L);

            var response = await EmployerCommitmentOrchestrator.GetSubmitNewCommitmentModel(
                HashedAccountId,
                externalUserId,
                transferConnectionCode,
                legalEntityCode,
                legalEntityName,
                legalEntityAddress,
                legalEntitySource,
                accountLegalEntityPublicHashedId,
                providerId,
                providerName,
                cohortRef,
                saveStatus);

            Assert.AreEqual(HashedAccountId, response.Data.HashedAccountId);
            Assert.AreEqual(transferConnectionCode, response.Data.TransferConnectionCode);
            Assert.AreEqual(legalEntityCode, response.Data.LegalEntityCode);
            Assert.AreEqual(legalEntityName, response.Data.LegalEntityName);
            Assert.AreEqual(legalEntityAddress, response.Data.LegalEntityAddress);
            Assert.AreEqual(legalEntitySource, response.Data.LegalEntitySource);
            Assert.AreEqual(accountLegalEntityPublicHashedId, response.Data.AccountLegalEntityPublicHashedId);
            Assert.AreEqual(providerId, response.Data.ProviderId);
            Assert.AreEqual(providerName, response.Data.ProviderName);
            Assert.AreEqual(cohortRef, response.Data.CohortRef);
            Assert.AreEqual(saveStatus, response.Data.SaveStatus);
        }
    }
}