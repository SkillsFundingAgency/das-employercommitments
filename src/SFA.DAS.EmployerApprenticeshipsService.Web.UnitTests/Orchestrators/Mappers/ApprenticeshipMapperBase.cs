using MediatR;

using Moq;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    public class ApprenticeshipMapperBase
    {
        protected Mock<IMediator> MockMediator;
        protected ApprenticeshipMapper Sut;
        protected Mock<ICurrentDateTime> MockDateTime;

        protected Mock<IAcademicYearValidator> AcademicYearValidator;

        public ApprenticeshipMapperBase()
        {
            SetUp();
        }

        public void SetUp()
        {
            var mockHashingService = new Mock<IHashingService>();

            AcademicYearValidator = new Mock<IAcademicYearValidator>();
            MockDateTime = new Mock<ICurrentDateTime>();
            MockMediator = new Mock<IMediator>();

            Sut = new ApprenticeshipMapper(mockHashingService.Object, MockDateTime.Object, MockMediator.Object, Mock.Of<ILog>(), AcademicYearValidator.Object);
        }

    }
}
