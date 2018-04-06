using System;
using System.Collections.Generic;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers.CommitmentMapperTests
{
    [TestFixture]
    public class WhenMappingToTransferCommitmentViewModel
    {
        private CommitmentMapper _sut;
        private Mock<IHashingService> _hashingService;
        private Mock<ICommitmentStatusCalculator> _commitmentStatusCalculator;
        private Mock<IFeatureToggleService> _featureToggleService;
        private Mock<IFeatureToggle> _featureToggle;
        private CommitmentView _commitmentView;
        private TransferRequest _transferRequest;


        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long p) => $"XYZ{p}");

            _commitmentStatusCalculator = new Mock<ICommitmentStatusCalculator>();

            _commitmentView = new CommitmentView
            {
                Id = 789,
                EmployerAccountId = 123,
                LegalEntityName = "LegalEntityName",
                TransferSender = new TransferSender
                {
                    Id = 1000,
                    Name = "Sender 1000",
                    TransferApprovalStatus = TransferApprovalStatus.Approved,
                    TransferApprovalSetBy = "tester",
                    TransferApprovalSetOn = new DateTime(2018, 3, 1)
                },
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

            _transferRequest = new TransferRequest
            {
                TransferRequestId = 789,
                CommitmentId = 876,
                ReceivingEmployerAccountId = 123,
                LegalEntityName = "LegalEntityName",
                SendingEmployerAccountId = 3434,
                Status = TransferApprovalStatus.Approved,
                ApprovedOrRejectedByUserName    = "tester",
                ApprovedOrRejectedByUserEmail    = "tester@test.com",
                ApprovedOrRejectedOn = new DateTime(2018, 3, 1),
                TransferCost = 10999m,
                TrainingList = new List<TrainingCourseSummary>
                {
                    new TrainingCourseSummary
                    {
                        CourseTitle = "Course1",
                        ApprenticeshipCount = 2
                    },
                    new TrainingCourseSummary
                    {
                        CourseTitle = "Course2",
                        ApprenticeshipCount = 21
                    }
                }
            };

            _featureToggleService = new Mock<IFeatureToggleService>();
            _featureToggle = new Mock<IFeatureToggle>();
            _featureToggleService.Setup(x => x.Get<TransfersRejectOption>()).Returns(_featureToggle.Object);

            _sut = new CommitmentMapper(_hashingService.Object, _commitmentStatusCalculator.Object, _featureToggleService.Object);
        }

        [TestCase(TransferApprovalStatus.Approved, "Approved")]
        [TestCase(TransferApprovalStatus.Rejected, "Rejected")]
        [TestCase(TransferApprovalStatus.Pending, "Pending")]
        public void ThenMappingACommitmentWithAppenticesMapsFieldsCorrectly(TransferApprovalStatus status, string statusDescription)
        {
            _commitmentView.TransferSender.TransferApprovalStatus = status;
            var result = _sut.MapToTransferCommitmentViewModel(_commitmentView);
            Assert.AreEqual($"XYZ{_commitmentView.TransferSender.Id}", result.HashedTransferSenderAccountId);
            Assert.AreEqual($"XYZ{_commitmentView.Id}", result.HashedCohortReference);
            Assert.AreEqual("LegalEntityName", result.LegalEntityName);
            Assert.AreEqual(1300m, result.TotalCost);
            Assert.AreEqual(2, result.TrainingList.Count);
            Assert.AreEqual("Name", result.TrainingList[0].CourseTitle);
            Assert.AreEqual(2, result.TrainingList[0].ApprenticeshipCount);
            Assert.AreEqual("Name (2 Apprentices)", result.TrainingList[0].SummaryDescription);
            Assert.AreEqual("Name2", result.TrainingList[1].CourseTitle);
            Assert.AreEqual(1, result.TrainingList[1].ApprenticeshipCount);
            Assert.AreEqual("Name2 (1 Apprentices)", result.TrainingList[1].SummaryDescription);
            Assert.AreEqual(statusDescription, result.TransferApprovalStatusDesc);
            Assert.AreEqual("tester", result.TransferApprovalSetBy);
            Assert.AreEqual(_commitmentView.TransferSender.TransferApprovalSetOn, result.TransferApprovalSetOn);

        }

        [TestCase(TransferApprovalStatus.Approved, "Approved")]
        [TestCase(TransferApprovalStatus.Rejected, "Rejected")]
        [TestCase(TransferApprovalStatus.Pending, "Pending")]
        public void ThenMappingATransferRequestWithAppenticesMapsFieldsCorrectly(TransferApprovalStatus status, string statusDescription)
        {
            _transferRequest.Status = status;
            var result = _sut.MapToTransferRequestViewModel(_transferRequest);
            Assert.AreEqual($"XYZ{_transferRequest.SendingEmployerAccountId}", result.HashedTransferSenderAccountId);
            Assert.AreEqual($"XYZ{_transferRequest.CommitmentId}", result.HashedCohortReference);
            Assert.AreEqual("LegalEntityName", result.LegalEntityName);
            Assert.AreEqual(10999m, result.TotalCost);
            Assert.AreEqual(2, result.TrainingList.Count);
            Assert.AreEqual("Course1", result.TrainingList[0].CourseTitle);
            Assert.AreEqual(2, result.TrainingList[0].ApprenticeshipCount);
            Assert.AreEqual("Course1 (2 Apprentices)", result.TrainingList[0].SummaryDescription);
            Assert.AreEqual("Course2", result.TrainingList[1].CourseTitle);
            Assert.AreEqual(21, result.TrainingList[1].ApprenticeshipCount);
            Assert.AreEqual("Course2 (21 Apprentices)", result.TrainingList[1].SummaryDescription);
            Assert.AreEqual(statusDescription, result.TransferApprovalStatusDesc);
            Assert.AreEqual("tester", result.TransferApprovalSetBy);
            Assert.AreEqual(_commitmentView.TransferSender.TransferApprovalSetOn, result.TransferApprovalSetOn);

        }


        [Test]
        public void ThenMappingACommitmentWithNoAppenticesProducesAnEmptyApprenticeSummary()
        {
            _commitmentView.Apprenticeships = null;
            var result = _sut.MapToTransferCommitmentViewModel(_commitmentView);
            Assert.AreEqual(0, result.TrainingList.Count);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ThenRejectionEnabledIfFeatureToggledOn(bool featureToggleEnabled, bool expectEnabled)
        {
            //Arrange
            _featureToggle.Setup(x => x.FeatureEnabled).Returns(featureToggleEnabled);

            //Assert
            var result = _sut.MapToTransferCommitmentViewModel(_commitmentView);
            Assert.AreEqual(expectEnabled, result.EnableRejection);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ThenRejectionEnabledIfFeatureToggledOnForTransferRequest(bool featureToggleEnabled, bool expectEnabled)
        {
            //Arrange
            _featureToggle.Setup(x => x.FeatureEnabled).Returns(featureToggleEnabled);

            //Assert
            var result = _sut.MapToTransferRequestViewModel(_transferRequest);
            Assert.AreEqual(expectEnabled, result.EnableRejection);
        }

    }
}