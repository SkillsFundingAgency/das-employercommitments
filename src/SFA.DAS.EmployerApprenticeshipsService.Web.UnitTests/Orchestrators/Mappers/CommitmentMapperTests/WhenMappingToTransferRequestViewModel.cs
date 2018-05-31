using System;
using System.Collections.Generic;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers.CommitmentMapperTests
{
    [TestFixture]
    public class WhenMappingToTransferRequestViewModel
    {
        private CommitmentMapper _sut;
        private Mock<IHashingService> _hashingService;
        private Mock<IPublicHashingService> _publicHashingService;
        private Mock<IFeatureToggleService> _featureToggleService;
        private Mock<IFeatureToggle> _featureToggle;
        private TransferRequest _transferRequest;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long p) => $"PRI{p}");
            _publicHashingService = new Mock<IPublicHashingService>();
            _publicHashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long p) => $"PUB{p}");

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

            _sut = new CommitmentMapper(_hashingService.Object, _featureToggleService.Object, _publicHashingService.Object);
        }

        [TestCase(TransferApprovalStatus.Approved, "Approved")]
        [TestCase(TransferApprovalStatus.Rejected, "Rejected")]
        [TestCase(TransferApprovalStatus.Pending, "Pending")]
        public void ThenMappingATransferRequestWithAppenticesMapsFieldsCorrectly(TransferApprovalStatus status, string statusDescription)
        {
            _transferRequest.Status = status;
            var result = _sut.MapToTransferRequestViewModel(_transferRequest);
            Assert.AreEqual($"PRI{_transferRequest.SendingEmployerAccountId}", result.HashedTransferSenderAccountId);
            Assert.AreEqual($"PUB{_transferRequest.SendingEmployerAccountId}", result.PublicHashedTransferSenderAccountId);
            Assert.AreEqual($"PRI{_transferRequest.ReceivingEmployerAccountId}", result.HashedTransferReceiverAccountId);
            Assert.AreEqual($"PUB{_transferRequest.ReceivingEmployerAccountId}", result.PublicHashedTransferReceiverAccountId);
            Assert.AreEqual($"PRI{_transferRequest.CommitmentId}", result.HashedCohortReference);
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
            Assert.AreEqual(new DateTime(2018, 3, 1), result.TransferApprovalSetOn);
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