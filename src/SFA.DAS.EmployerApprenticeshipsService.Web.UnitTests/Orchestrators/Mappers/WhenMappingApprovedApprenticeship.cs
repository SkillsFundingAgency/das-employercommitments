using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingApprovedApprenticeship
    {
        private ApprovedApprenticeshipMapper _mapper;
        private ApprovedApprenticeship _source;
        private Mock<ICurrentDateTime> _currentDateTime;

        [SetUp]
        public void Arrange()
        {
            _currentDateTime = new Mock<ICurrentDateTime>();
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 6, 1));

            _mapper = new ApprovedApprenticeshipMapper(_currentDateTime.Object);

            _source = new ApprovedApprenticeship
            {
                Id = 1,
                EndpointAssessorName = "EPA",
                HasHadDataLockSuccess = false,
                AccountLegalEntityPublicHashedId = "AGREEMENT_ID",
                LegalEntityName = "TEST_LEGAL_ENTITY_NAME",
                LegalEntityId = "TEST_LEGAL_ENTITY_ID",
                ProviderId = 3,
                ProviderName = "TEST_PROVIDER_NAME",
                UpdateOriginator = Originator.Employer,
                PaymentOrder = 2,
                ProviderRef = "PROVIDER_REF",
                EmployerRef = "EMPLOYER_REF",
                PaymentStatus = PaymentStatus.Active,
                StopDate = null,
                PauseDate = null,
                StartDate = new DateTime(2018, 1, 1),
                EndDate = new DateTime(2020, 12, 31),
                TrainingName = "TRAINING_NAME",
                TrainingCode = "TRAINING_CODE",
                TrainingType = TrainingType.Framework,
                ULN = "ULN",
                DateOfBirth = new DateTime(2000, 1, 1),
                LastName = "LAST_NAME",
                FirstName = "FIRST_NAME",
                TransferSenderId = null,
                EmployerAccountId = 4,
                CohortReference = "COHORT_REF"
            };
        }

        [Test]
        public void ThenAllPropertiesAreMappedCorrectly()
        {
            var result = _mapper.Map(TestHelper.Clone(_source));

            Assert.AreEqual(_source.AccountLegalEntityPublicHashedId, result.AccountLegalEntityPublicHashedId);
            Assert.AreEqual(_source.FirstName, result.FirstName);
            Assert.AreEqual(_source.LastName, result.LastName);
            Assert.AreEqual(_source.ULN, result.ULN);
            Assert.AreEqual(_source.DateOfBirth, result.DateOfBirth);
            Assert.AreEqual(_source.StartDate, result.StartDate);
            Assert.AreEqual(_source.EndDate, result.EndDate);
            Assert.AreEqual(_source.StopDate, result.StopDate);
            Assert.AreEqual(_source.TrainingType, result.TrainingType);
            Assert.AreEqual(_source.TrainingName, result.TrainingName);
            Assert.AreEqual(_source.PaymentStatus, result.PaymentStatus);
            Assert.AreEqual(_source.ProviderName, result.ProviderName);
            Assert.AreEqual(_source.EmployerRef, result.EmployerReference);
            Assert.AreEqual(_source.CohortReference, result.CohortReference);
            Assert.AreEqual(_source.EndpointAssessorName, result.EndpointAssessorName);
        }

        [TestCase(TriageStatus.Restart, true)]
        [TestCase(TriageStatus.Change, false)]
        [TestCase(TriageStatus.Unknown, false)]
        public void ThenPendingDataLockRestartIsMappedCorrectly(TriageStatus triageStatus, bool expectPendingRestart)
        {
            //Arrange
            _source.DataLocks = new List<DataLockStatus>
            {
                new DataLockStatus
                {
                    ErrorCode = DataLockErrorCode.Dlock03,
                    Status = Status.Fail,
                    TriageStatus = triageStatus
                }
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectPendingRestart, result.PendingDataLockRestart);
        }

        [TestCase(TriageStatus.Change, true)]
        [TestCase(TriageStatus.Unknown, false)]
        public void ThenPendingDataLockChangeIsMappedCorrectly(TriageStatus triageStatus, bool expectPendingChange)
        {
            //Arrange
            _source.DataLocks = new List<DataLockStatus>
            {
                new DataLockStatus
                {
                    ErrorCode = DataLockErrorCode.Dlock07,
                    Status = Status.Fail,
                    TriageStatus = triageStatus
                }
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectPendingChange, result.PendingDataLockChange);
        }


        [TestCase(PaymentStatus.Active, true)]
        [TestCase(PaymentStatus.Withdrawn, false)]
        public void ThenCanEditStatusIsMappedCorrectly(PaymentStatus paymentStatus, bool expectCanEditStatus)
        {
            //Arrange
            _source.PaymentStatus = paymentStatus;

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectCanEditStatus, result.CanEditStatus);
        }

        [TestCase(PaymentStatus.Withdrawn, false, true)]
        [TestCase(PaymentStatus.Active, false, false)]
        [TestCase(PaymentStatus.Withdrawn, true, false)]
        [TestCase(PaymentStatus.Paused, false, false)]
        public void ThenCanEditStopDateIsMappedCorrectly(PaymentStatus paymentStatus, bool stoppedBackToStart, bool expectCanEditStopDate)
        {
            //Arrange
            _source.PaymentStatus = paymentStatus;
            if (stoppedBackToStart) _source.StopDate = _source.StartDate;

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectCanEditStopDate, result.CanEditStopDate);
        }

        [Test]
        public void ThenEditIsEnabledIfAllConditionsMet()
        {
            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(true, result.EnableEdit);
        }

        [Test]
        public void ThenEditIsDisabledIfCourseDataLockTriagedAsChange()
        {
            //Arrange
            _source.DataLocks = new List<DataLockStatus>
            {
                new DataLockStatus
                {
                    ErrorCode = DataLockErrorCode.Dlock03,
                    TriageStatus = TriageStatus.Change
                }
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.IsFalse(result.EnableEdit);
        }

        [Test]
        public void ThenEditIsDisabledIfCourseDataLockTriagedAsRestart()
        {
            //Arrange
            _source.DataLocks = new List<DataLockStatus>
            {
                new DataLockStatus
                {
                    ErrorCode = DataLockErrorCode.Dlock03,
                    TriageStatus = TriageStatus.Restart
                }
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.IsFalse(result.EnableEdit);
        }

        [Test]
        public void ThenEditIsDisabledIfPriceDataLockTriagedAsChange()
        {
            //Arrange
            _source.DataLocks = new List<DataLockStatus>
            {
                new DataLockStatus
                {
                    ErrorCode = DataLockErrorCode.Dlock07,
                    TriageStatus = TriageStatus.Change
                }
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.IsFalse(result.EnableEdit);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ThenEditDisabledIfPendingChanges(bool hasPendingChanges, bool expectCanEdit)
        {
            //Arrange
            if (hasPendingChanges) _source.PendingUpdate = new ApprenticeshipUpdate();

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectCanEdit, result.EnableEdit);
        }

        [TestCase(PaymentStatus.Active, true)]
        [TestCase(PaymentStatus.Paused, true)]
        [TestCase(PaymentStatus.Withdrawn, false)]
        public void ThenEditDisabledIfNotActiveOrPaused(PaymentStatus paymentStatus, bool expectCanEdit)
        {
            //Arrange
            _source.PaymentStatus = paymentStatus;

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectCanEdit, result.EnableEdit);
        }

        [Test]
        public void ThenCurrentCostIsMappedCorrectly()
        {
            //Arrange
            _source.PriceEpisodes = new List<PriceHistory>
            {
                new PriceHistory{ Cost = 1000, FromDate = new DateTime(2018,1,1), ToDate = new DateTime(2019,1,1)}
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(1000, result.CurrentCost);
        }

        [TestCase("2018-01-01", "2018-06-01", PaymentStatus.Active, "Waiting to start")]
        [TestCase("2018-06-01", "2018-06-01", PaymentStatus.Active, "Live")]
        [TestCase("2018-01-01", "2018-06-01", PaymentStatus.Withdrawn, "Stopped")]
        [TestCase("2018-01-01", "2018-06-01", PaymentStatus.Paused, "Paused")]
        public void ThenStatusIsMappedCorrectly(DateTime now, DateTime startDate, PaymentStatus paymentStatus, string expectedStatus)
        {
            //Arrange
            _currentDateTime.Setup(x => x.Now).Returns(now);
            _source.PaymentStatus = paymentStatus;
            _source.StartDate = startDate;

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectedStatus, result.Status);
        }

        [TestCase(Originator.Employer, PendingChanges.WaitingForApproval)]
        [TestCase(Originator.Provider, PendingChanges.ReadyForApproval)]
        public void ThenPendingChangesIsMappedCorrectly(Originator originator, PendingChanges expectPendingChange)
        {
            //Arrange
            _source.PendingUpdate = new ApprenticeshipUpdate
            {
                Originator = originator
            };

            //Act
            var result = _mapper.Map(TestHelper.Clone(_source));

            //Assert
            Assert.AreEqual(expectPendingChange, result.PendingChanges);

        }

    }
}
