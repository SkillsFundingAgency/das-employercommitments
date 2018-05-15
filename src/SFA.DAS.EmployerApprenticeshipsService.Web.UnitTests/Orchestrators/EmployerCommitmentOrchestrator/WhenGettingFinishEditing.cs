using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Application.Exceptions;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingFinishEditing : OrchestratorTestBase
    {
        private CommitmentView _commitmentView;

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView
            {
                Id = 123,
                LegalEntityId = "123",
                EditStatus = EditStatus.EmployerOnly
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = _commitmentView
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
                    LegalEntities = new List<LegalEntity> { new LegalEntity { Code = "123", Agreements = new List<Agreement>() } }
                });

            //Act
            await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c => c.HashedAccountId == "ABC123" && c.UserId== "XYZ123")), Times.Once);
        }

        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Pending, true)]
        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Removed, EmployerAgreementStatus.Signed, true)]
        [TestCase(null, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Removed, EmployerAgreementStatus.Pending, false)]
        [TestCase(null, EmployerAgreementStatus.Pending, false)]
        public async Task ThenHasSignedAgreementIsCorrectlyDeterminedForNonTransfers(EmployerAgreementStatus v1,
            EmployerAgreementStatus v2, bool expectHasSigned)
        {
            //Arrange
            _commitmentView.TransferSender = null;

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(MockLegalEntitiesResponse(v1,v2));

            //Act
            var result = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(expectHasSigned, result.Data.HasSignedTheAgreement);
        }

        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Removed, EmployerAgreementStatus.Signed, true)]
        [TestCase(null, EmployerAgreementStatus.Signed, true)]
        [TestCase(EmployerAgreementStatus.Removed, EmployerAgreementStatus.Pending, false)]
        [TestCase(EmployerAgreementStatus.Signed, EmployerAgreementStatus.Pending, false)]
        [TestCase(null, EmployerAgreementStatus.Pending, false)]
        public async Task ThenHasSignedAgreementIsCorrectlyDeterminedForTransfers(EmployerAgreementStatus v1,
            EmployerAgreementStatus v2, bool expectHasSigned)
        {
            //Arrange
            _commitmentView.TransferSender = new TransferSender {Id = 1};

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(MockLegalEntitiesResponse(v1, v2));

            //Act
            var result = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(expectHasSigned, result.Data.HasSignedTheAgreement);
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
                        Agreements = new List<Agreement>
                        {
                            new Agreement { TemplateVersionNumber = 2, Status = EmployerAgreementStatus.Signed }
                        }
                    } }
                });

            //Act
            var actual = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        private GetAccountLegalEntitiesResponse MockLegalEntitiesResponse(EmployerAgreementStatus? v1,
            EmployerAgreementStatus? v2)
        {
            var agreements = new List<Agreement>();

            if (v1.HasValue)
            {
                agreements.Add( new Agreement {TemplateVersionNumber = 1, Status = v1.Value } );
            }

            if (v2.HasValue)
            {
                agreements.Add(new Agreement { TemplateVersionNumber = 2, Status = v2.Value });
            }

            return new GetAccountLegalEntitiesResponse
            {
                LegalEntities = new List<LegalEntity>
                {
                    new LegalEntity
                    {
                        Code = "123",
                        Agreements = agreements
                    }
                }
            };
        }

    }
}
