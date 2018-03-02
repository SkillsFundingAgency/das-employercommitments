using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingEditApprenticeshipStopDateViewModel : ApprenticeshipMapperBase
    {
        [Test]
        public void ThenIfR14HasPassedThenAcademicYearRestrictionIsSetToCurrentAcademicYearStartDateForApprenticeshipsStartedLastAcademicYear()
        {
            //Arrange
            MockDateTime = new Mock<ICurrentDateTime>();
            MockDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 2, 1));

            var apprenticeship = new Apprenticeship
            {
                StartDate = new DateTime(2017,6,1),
                StopDate = new DateTime(2018,2,1)
            };

            var academicYearStartDate = new DateTime(2017, 8, 1);

            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(academicYearStartDate);

            Sut = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object, MockMediator.Object,
                Mock.Of<ILog>(), AcademicYearValidator.Object, AcademicYearDateProvider.Object);

            //Act
            var result = Sut.MapToEditApprenticeshipStopDateViewModel(apprenticeship);

            //Assert
            Assert.AreEqual(academicYearStartDate, result.AcademicYearRestriction);
        }

        [Test]
        public void ThenIfR14HasNotPassedThenThereIsNoAcademicYearRestriction()
        {
            //Arrange
            MockDateTime = new Mock<ICurrentDateTime>();
            MockDateTime.Setup(x => x.Now).Returns(new DateTime(2017, 8, 15));

            var apprenticeship = new Apprenticeship
            {
                StartDate = new DateTime(2018, 6, 1),
                StopDate = new DateTime(2018, 2, 1)
            };

            var academicYearStartDate = new DateTime(2017, 8, 1);
            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(academicYearStartDate);

            Sut = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object, MockMediator.Object,
                Mock.Of<ILog>(), AcademicYearValidator.Object, AcademicYearDateProvider.Object);

            //Act
            var result = Sut.MapToEditApprenticeshipStopDateViewModel(apprenticeship);

            //Assert
            Assert.IsNull(result.AcademicYearRestriction);
        }
    }
}
