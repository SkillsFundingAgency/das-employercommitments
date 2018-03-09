using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingToTransferCommitmentViewModel
    {
        private CommitmentMapper _sut;
        private Mock<IHashingService> _hashingService;
        private Mock<ICommitmentStatusCalculator> _commitmentStatusCalculator;
        private CommitmentView _commitmentView;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(789)).Returns("XYZ789");
            _hashingService.Setup(x => x.HashValue(1000)).Returns("DEF1000");

            _commitmentStatusCalculator = new Mock<ICommitmentStatusCalculator>();

            _commitmentView = new CommitmentView
            {
                Id = 789,
                EmployerAccountId = 123,
                LegalEntityName = "LegalEntityName",
                TransferSenderId = 1000,
                TransferSenderName = "Sender 1000",
                Apprenticeships = new List<Apprenticeship>
                {
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
            };

            _sut = new CommitmentMapper(_hashingService.Object, _commitmentStatusCalculator.Object);
        }

        [Test]
        public void ThenMappingACommitmentWithAppenticesMapsFieldsCorrectly()
        {
            var result = _sut.MapToTransferCommitmentViewModel(_commitmentView);
            Assert.AreEqual("DEF1000", result.HashedTransferSenderAccountId);
            Assert.AreEqual("XYZ789", result.HashedCohortReference);
            Assert.AreEqual("LegalEntityName", result.LegalEntityName);
            Assert.AreEqual(1300m, result.TotalCost);
            Assert.AreEqual(2, result.TrainingList.Count);
            Assert.AreEqual("Name", result.TrainingList[0].CourseTitle);
            Assert.AreEqual(2, result.TrainingList[0].ApprenticeshipCount);
            Assert.AreEqual("Name (2 Apprentices)", result.TrainingList[0].SummaryDescription);
            Assert.AreEqual("Name2", result.TrainingList[1].CourseTitle);
            Assert.AreEqual(1, result.TrainingList[1].ApprenticeshipCount);
            Assert.AreEqual("Name2 (1 Apprentices)", result.TrainingList[1].SummaryDescription);

        }
        [Test]
        public void ThenMappingACommitmentWithNoAppenticesProducesAnEmptyApprenticeSummary()
        {
            _commitmentView.Apprenticeships = null;
            var result = _sut.MapToTransferCommitmentViewModel(_commitmentView);
            Assert.AreEqual(0, result.TrainingList.Count);
        }
    }
}