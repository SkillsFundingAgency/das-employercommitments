﻿using System;
using MediatR;

using Moq;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    public class ApprenticeshipMapperBase
    {
        protected Mock<IMediator> MockMediator;
        protected ApprenticeshipMapper Sut;
        protected Mock<ICurrentDateTime> MockDateTime;
        protected Mock<IAcademicYearValidator> AcademicYearValidator;
        protected Mock<IAcademicYearDateProvider> AcademicYearDateProvider;

        public ApprenticeshipMapperBase()
        {
            SetUp();
        }

        public void SetUp()
        {
            var mockHashingService = new Mock<IHashingService>();

            AcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod)
                .Returns(new DateTime(2017, 9, 30));

            AcademicYearValidator = new Mock<IAcademicYearValidator>();

            MockDateTime = new Mock<ICurrentDateTime>();
            MockDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 3, 1));

            MockMediator = new Mock<IMediator>();
            
            Sut = new ApprenticeshipMapper(mockHashingService.Object, MockDateTime.Object, MockMediator.Object,
                Mock.Of<ILog>(), AcademicYearValidator.Object, AcademicYearDateProvider.Object);
        }

    }
}
