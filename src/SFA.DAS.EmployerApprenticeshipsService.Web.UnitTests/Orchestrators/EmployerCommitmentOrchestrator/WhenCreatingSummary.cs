using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProvider;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenCreatingSummary : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldReturnCorrectCreateCommitmentViewModel()
        {
            const string hashedAccountId = "HASHACC";
            const string transferConnectionCode = "TCC";
            const string legalEntityCode = "LECODE";
            const long providerId = 123L;
            const string cohortRef = "COREF";
            const string externalUserId = "USERID";
            const long ukprn = 321L;
            const string providerName = "Pro Name";
            const string legalEntityName = "LE Name";
            const string legalEntityAddress = "LE Address";
            const short legalEntitySourse = 1;
            const string accountLegalEntityPublicHashedId = "123456";

            MockMediator.Setup(m => m.SendAsync(It.Is<GetProviderQueryRequest>(r => r.ProviderId == providerId)))
                .ReturnsAsync(new GetProviderQueryResponse
                {
                    ProvidersView =
                        new ProvidersView {Provider = new Provider {Ukprn = ukprn, ProviderName = providerName}}
                });

            MockMediator.Setup(m => m.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(
                    r => r.HashedAccountId == hashedAccountId && r.UserId == externalUserId)))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                    {
                        LegalEntities = new List<LegalEntity>
                        {
                            new LegalEntity
                            {
                                Code = legalEntityCode,
                                Name = legalEntityName,
                                RegisteredAddress = legalEntityAddress,
                                Source = legalEntitySourse,
                                AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId
                            }
                        }
                    });

            var response = await EmployerCommitmentOrchestrator.CreateSummary(
                hashedAccountId, transferConnectionCode, legalEntityCode, providerId.ToString(),
                cohortRef, externalUserId);

            Assert.AreEqual(hashedAccountId, response.Data.HashedAccountId);
            Assert.AreEqual(transferConnectionCode, response.Data.TransferConnectionCode);
            Assert.AreEqual(legalEntityCode, response.Data.LegalEntityCode);
            Assert.AreEqual(legalEntityName, response.Data.LegalEntityName);
            Assert.AreEqual(legalEntityAddress, response.Data.LegalEntityAddress);
            Assert.AreEqual(legalEntitySourse, response.Data.LegalEntitySource);
            Assert.AreEqual(accountLegalEntityPublicHashedId, response.Data.AccountLegalEntityPublicHashedId);
            Assert.AreEqual(ukprn, response.Data.ProviderId);
            Assert.AreEqual(providerName, response.Data.ProviderName);
            Assert.AreEqual(cohortRef, response.Data.CohortRef);
        }
    }
}
