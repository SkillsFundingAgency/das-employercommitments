using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Application;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingFinishEditing : OrchestratorTestBase
    {
        
        [SetUp]
        public void Arrange()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Id = 123,
                        LegalEntityId = "321",
                        EditStatus = EditStatus.EmployerOnly
                    }
                });
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(
                    new GetOverlappingApprenticeshipsQueryResponse
                    {
                        Overlaps = Enumerable.Empty<ApprenticeshipOverlapValidationResult>()
                    });
        }

        [Test]
        public async Task ShouldCallMediatorToGetLegalEntityAgreementRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity> { new LegalEntity { Code = "321" } }
                });

            //Act
            await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c => c.HashedAccountId == "ABC123" && c.UserId== "XYZ123")), Times.Once);
        }

        [TestCase(true, Description = "The Employer has signed the agreement")]
        [TestCase(false, Description = "The Employer has not signed the agreement")]
        public async Task ThenTheViewModelShouldReflectWhetherTheAgreementHasBeenSigned(bool isSigned)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity> { new LegalEntity
                    {
                        Code = "321",
                        AgreementStatus = isSigned ? EmployerAgreementStatus.Signed : EmployerAgreementStatus.Pending
                    } }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(isSigned, result.Data.HasSignedTheAgreement);
        }

        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownThenTheOrchestratorResponseIsPopulatedAsBadRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "" } }));

            //Act
            var actual = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.AreEqual(1, actual.FlashMessage.ErrorMessages.Count);
        }

        [Test]
        public async Task ThenIfTheCodeDoesNotExistTheResponseIsPopulatedAsBadRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity> { new LegalEntity
                    {
                        Code = "XYZ1233",
                        AgreementStatus = EmployerAgreementStatus.Signed
                    } }
                });

            //Act
            var actual = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }
    }
}
