using MediatR;
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
    public class WhenMappingApprenticeships
    {
        private IApprenticeshipMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var hashingService = new Mock<IHashingService>();
            hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);

            var dateTime = new Mock<ICurrentDateTime>();
            var mediator = new Mock<IMediator>();
            var log = new Mock<ILog>();
            var validator = new Mock<IAcademicYearValidator>();

            _mapper = new ApprenticeshipMapper(hashingService.Object, dateTime.Object, mediator.Object, log.Object, validator.Object);
        }

        [Test]
        public void ThenMappingAnApprenticeshipUpdateShouldReturnAResult()
        {
            var update = new ApprenticeshipUpdate();

            Assert.DoesNotThrow(() => _mapper.MapFrom(update));

        }

        [Test]
        public void TheMappingAnEmptyUpdateShouldReturnNull()
        {
            Assert.IsNull(_mapper.MapFrom((ApprenticeshipUpdate)null));

        }
    }
}