﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDetails;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetApprenticeshipDetails
{
    class WhenIGetApprenticeshipDetails : QueryBaseTest<GetApprenticeshipDetailsHandler, GetApprenticeshipDetailsQuery, GetApprenticeshipDetailsResponse>
    {
        private Mock<IApprenticeshipInfoService> _apprenticeshipInfoService;
        private EmployerCommitments.Domain.Models.ApprenticeshipProvider.Provider _provider;
        
        public override GetApprenticeshipDetailsQuery Query { get; set; }
        public override GetApprenticeshipDetailsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetApprenticeshipDetailsQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _provider = new EmployerCommitments.Domain.Models.ApprenticeshipProvider.Provider
            {
                Name = "Test Provider"
            };

            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoService>();

            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(new ProvidersView
            {
                CreatedDate = DateTime.Now,
                Provider = _provider
            });

            Query = new GetApprenticeshipDetailsQuery
            {
                ApprenticeshipId = 1,
                ProviderId = 12
            };

            RequestHandler = new GetApprenticeshipDetailsHandler(RequestValidator.Object, _apprenticeshipInfoService.Object);
        }

        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetProvider(Query.ProviderId), Times.Once);
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_provider.Name, result.ProviderName);
        }

        [Test]
        public async Task ThenIfNoProvidersAreReturnsAnUnknownProviderLabelShouldBeAdded()
        {
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(new ProvidersView
            {
                CreatedDate = DateTime.Now,
                Provider = new EmployerCommitments.Domain.Models.ApprenticeshipProvider.Provider()
            });

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Unknown provider", result.ProviderName);
        }

        [Test]
        public async Task ThenIfNoProviderIsFoundUnknownProviderLabelShouldBeAdded()
        {
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(() => null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Unknown provider", result.ProviderName);
        }
    }
}
