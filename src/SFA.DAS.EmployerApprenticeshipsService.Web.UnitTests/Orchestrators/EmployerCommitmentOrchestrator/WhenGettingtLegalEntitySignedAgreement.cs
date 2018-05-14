using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
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
                .ReturnsAsync(MockLegalEntitiesResponse(EmployerAgreementStatus.Removed, EmployerAgreementStatus.Signed));

            //Act
            await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "123", "C789","123EDC");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c => c.HashedAccountId == "ABC123" && c.UserId == "123EDC")), Times.Once);
        }

        //[TestCase(true, Description = "The Employer has signed the agreement")]
        //[TestCase(false, Description = "The Employer has not signed the agreement")]
        //public async Task ThenTheViewModelShouldReflectWhetherTheAgreementHasBeenSigned(bool isSigned)
        //{
        //    //Arrange
        //    MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
        //        .ReturnsAsync(new GetAccountLegalEntitiesResponse
        //        {
        //            LegalEntities = new List<LegalEntity> { new LegalEntity
        //            {
        //                Code = "XYZ123",
        //                AgreementStatus = isSigned ? EmployerAgreementStatus.Signed : EmployerAgreementStatus.Pending
        //            } }
        //        });

        //    //Act
        //    var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "XYZ123", "C789", "123EDC");

        //    //Assert
        //    Assert.AreEqual(isSigned, result.Data.HasSignedAgreement);
        //}


        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Pending, true)]
        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Superseded, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Superseded, EmployerAgreementStatus.Pending, false)]
        public async Task ThenHasSignedAgreementIsCorrectlyDeterminedForNonTransfers(EmployerAgreementStatus v1,
            EmployerAgreementStatus v2, bool expectHasSigned)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(MockLegalEntitiesResponse(v1, v2));

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", null, "123", "C789", "123EDC");


            //Assert
            Assert.AreEqual(expectHasSigned, result.Data.HasSignedAgreement);

        }

        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Superseded, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Superseded, EmployerAgreementStatus.Pending, false)]
        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Pending, false)]
        public async Task ThenHasSignedAgreementIsCorrectlyDeterminedForTransfers(EmployerAgreementStatus v1,
            EmployerAgreementStatus v2, bool expectHasSigned)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(MockLegalEntitiesResponse(v1, v2));

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", "789FGH", "123", "C789", "123EDC");


            //Assert
            Assert.AreEqual(expectHasSigned, result.Data.HasSignedAgreement);
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
                        Agreements = new List<Agreement>
                        {
                            new Agreement { TemplateVersionNumber = 2, Status = EmployerAgreementStatus.Signed }
                        }
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
                    LegalEntities = new List<LegalEntity> { new LegalEntity { Code = "123", Agreements = new List<Agreement>()} }
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", "T1234", "123", "C789", "123EDC");

            //Assert
            result.Data.TransferConnectionCode.Should().Be("T1234");
        }


        private GetAccountLegalEntitiesResponse MockLegalEntitiesResponse(EmployerAgreementStatus v1,
            EmployerAgreementStatus v2)
        {
            var result = new GetAccountLegalEntitiesResponse
            {
                LegalEntities = new List<LegalEntity>
                {
                    new LegalEntity
                    {
                        Code = "123",
                        Agreements = new List<Agreement>
                        {
                            new Agreement { TemplateVersionNumber = 1, Status = v1 },
                            new Agreement { TemplateVersionNumber = 2, Status = v2 }
                        }
                    }
                }
            };

            return result;
        }
    }
}
