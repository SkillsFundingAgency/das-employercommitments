using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingCommitmentViewModelForTransferSender : OrchestratorTestBase
    {
        
        [SetUp]
        public void Arrange()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Id = 789,
                        EmployerAccountId = 123,
                        LegalEntityName = "LegalEntityName",
                        TransferSenderId = 1000,
                        TransferSenderName = "Sender 1000",
                        Apprenticeships = new List<Apprenticeship> {
                            new Apprenticeship
                            {
                                TrainingCode = "ABC",
                                TrainingName = "Name",
                                Cost = 100
                            },
                            new Apprenticeship
                            {
                                TrainingCode = "ABC",
                                TrainingName = "Name",
                                Cost = 200
                            },
                            new Apprenticeship
                            {
                                TrainingCode = "ABC2",
                                TrainingName = "Name2",
                                Cost = 1000
                            },

                        }
                    }
                });
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123);
            MockHashingService.Setup(x => x.DecodeValue("XYZ789")).Returns(789);
        }

        [Test]
        public async Task ShouldCallMediatorWithExpectedQuery()
        {
            //Act
            await EmployerCommitmentOrchestrator.GetCommitmentDetailsForTransfer("ABC123", "XYZ789", "UserA");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetCommitmentQueryRequest>(c =>
                c.AccountId == 123 && c.CommitmentId == 789 && c.CallType == CallType.TransferSender)));
        }

        [Test]
        public async Task ThenItShouldGroupTheApprenticeshipsAndMapProperties()
        {
            //Arrange 
            MockHashingService.Setup(x => x.HashValue(789)).Returns("XYZ789");
            MockHashingService.Setup(x => x.HashValue(1000)).Returns("DEF1000");

            //Act
            var actual = await EmployerCommitmentOrchestrator.GetCommitmentDetailsForTransfer("ABC123", "XYZ789", "UserA");

            //Assert
            Assert.AreEqual("DEF1000", actual.Data.HashedTransferSenderAccountId);
            Assert.AreEqual("XYZ789", actual.Data.HashedCohortReference);
            Assert.AreEqual("LegalEntityName", actual.Data.LegalEntityName);
            Assert.AreEqual(1300m, actual.Data.TotalCost);
            Assert.AreEqual(2, actual.Data.TrainingList.Count);
            Assert.AreEqual("Name", actual.Data.TrainingList[0].CourseTitle);
            Assert.AreEqual(2, actual.Data.TrainingList[0].ApprenticeshipCount);
            Assert.AreEqual("Name (2 Apprentices)", actual.Data.TrainingList[0].SummaryDescription);
            Assert.AreEqual("Name2", actual.Data.TrainingList[1].CourseTitle);
            Assert.AreEqual(1, actual.Data.TrainingList[1].ApprenticeshipCount);
            Assert.AreEqual("Name2 (1 Apprentices)", actual.Data.TrainingList[1].SummaryDescription);
        }

    }
}
