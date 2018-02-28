using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingtLegalEntitySignedAgreement : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToGetLegalEntityAgreementRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity> { new LegalEntity {Code="123"} }
                });

            //Act
            await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "123", "C789","123EDC");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c => c.HashedAccountId == "ABC123" && c.UserId == "123EDC")), Times.Once);
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
                        Code = "XYZ123",
                        AgreementStatus = isSigned ? EmployerAgreementStatus.Signed : EmployerAgreementStatus.Pending
                    } }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "XYZ123", "C789", "123EDC");

            //Assert
            Assert.AreEqual(isSigned, result.Data.HasSignedAgreement);
        }

        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownThenTheOrchestratorResponseIsPopulatedAsBadRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ThrowsAsync(new InvalidRequestException (new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "XYZ123", "C789", "123EDC");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
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
            var actual = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "XYZ123", "C789", "123EDC");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ShouldMapTransferConnectionCodeToResponse()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity> { new LegalEntity { Code = "123" } }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", "T1234", "123", "C789", "123EDC");

            //Assert
            result.Data.TransferConnectionCode.Should().Be("T1234");
        }

    }
}
