﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Http;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EmployerCommitments.Infrastructure.Data;
using SFA.DAS.EmployerCommitments.Infrastructure.ExecutionPolicies;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.ProviderEmailLookupService
{
    public class ProviderEmailLookupServiceTests
    {
        private Infrastructure.Services.ProviderEmailLookupService _sut;

        private readonly EmployerCommitmentsServiceConfiguration _config;
        private Mock<IdamsEmailServiceWrapper> _mockIdamsService;
        private Mock<IApprenticeshipInfoService> _mockApprenticeshipService;

        public ProviderEmailLookupServiceTests()
        {
            _config = new EmployerCommitmentsServiceConfiguration
                         {
                             CommitmentNotification =
                                 new CommitmentNotificationConfiguration
                                     {
                                         UseProviderEmail = false,
                                         ProviderTestEmails = new List<string> { "test@email.uk", "test2@email.uk" },
                                         IdamsListUsersUrl = "http://www.url.uk/path/to?query={0}"
                                     }
                         };
        }

        [SetUp]
        public void SetUp()
        {
            _mockIdamsService = new Mock<IdamsEmailServiceWrapper>
                (Mock.Of<ILog>(), _config, Mock.Of<IHttpClientWrapper>(), Mock.Of<ExecutionPolicy>());
            _mockIdamsService.Setup(m => m.GetEmailsAsync(It.IsAny<long>())).ReturnsAsync(new List<string>());
            _mockIdamsService.Setup(m => m.GetSuperUserEmailsAsync(It.IsAny<long>())).ReturnsAsync(new List<string>());

            _mockApprenticeshipService = new Mock<IApprenticeshipInfoService>();
            _sut = new Infrastructure.Services.ProviderEmailLookupService(
                Mock.Of<ILog>(),
                _mockIdamsService.Object,
                _config,
                _mockApprenticeshipService.Object
                );
        }

        [Test]
        public async Task WhenNoAddressesAreFound()
        {
            _config.CommitmentNotification.UseProviderEmail = true;

            var result = await _sut.GetEmailsAsync(123456L, string.Empty);
            result.Count.Should().Be(0);
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task WhenUsingATestEmail()
        {
            _config.CommitmentNotification.UseProviderEmail = false;

            var result = await _sut.GetEmailsAsync(123456L, string.Empty);
            result.Count.Should().Be(2);
            result[0].Should().Be("test@email.uk");
            result[1].Should().Be("test2@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenUsingAEmailFromLastUpdate()
        {
            _config.CommitmentNotification.UseProviderEmail = true;

            var result = await _sut.GetEmailsAsync(123456L, "lastUpdateProvider@email.uk");
            result.Count.Should().Be(1);
            result[0].Should().Be("lastUpdateProvider@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenAddressInIdamsForDasUser()
        {
            _config.CommitmentNotification.UseProviderEmail = true;
            _mockIdamsService.Setup(m => m.GetEmailsAsync(It.IsAny<long>())).ReturnsAsync(new List<string> { "idams@email.uk" });
            var result = await _sut.GetEmailsAsync(123456L, string.Empty);
            result.Count.Should().Be(1);
            result[0].Should().Be("idams@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenAddressInIdamsForSuperUser()
        {
            _config.CommitmentNotification.UseProviderEmail = true;
            _mockIdamsService.Setup(m => m.GetSuperUserEmailsAsync(It.IsAny<long>())).ReturnsAsync(new List<string> { "idamsSuperUser@email.uk" });
            var result = await _sut.GetEmailsAsync(123456L, string.Empty);
            result.Count.Should().Be(1);
            result[0].Should().Be("idamsSuperUser@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenAddressInProviderApi()
        {
            _config.CommitmentNotification.UseProviderEmail = true;
            _mockApprenticeshipService.Setup(m => m.GetProvider(It.IsAny<long>()))
                .Returns(
                    new ProvidersView
                        {
                            CreatedDate = DateTime.Now,
                            Provider = new Domain.Models.ApprenticeshipProvider.Provider { Email = "provider@email.uk" } 
                            
                        });
            var result = await _sut.GetEmailsAsync(123456L, string.Empty);
            result.Count.Should().Be(1);
            result[0].Should().Be("provider@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockIdamsService.Verify(m => m.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<long>()), Times.Once);
        }
    }
}
